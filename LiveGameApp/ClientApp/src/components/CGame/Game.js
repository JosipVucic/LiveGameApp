import React, { Component } from 'react';
import DataProvider from '../admin/DataProvider';
import { FilterPill } from '../CUtil/FilterPill';
import { Redirect } from 'react-router-dom';
import authService from '../api-authorization/AuthorizeService';

export class Game extends Component {
    static displayName = Game.name;

    constructor(props) {
        super(props);
        this.state = {
            pagination: {
                page: 1,
                perPage: 50,
            },
            reviewPagination: {
                page: 1,
                perPage: 12,
                hasPrev: false,
                hasNext: false,
            },
            item: undefined,
            editMode: false,
            writeMode: false,
            genres: [],
            selectedGenres: [],
            authors: [],
            selectedAuthors: [],
            redirect: false,

            reviews: [],
            review: {
                Rating: 5,
                Content: "",
            },

            isAuthenticated: false,
            userName: null,
            userId: null,
        };
    }

    componentDidMount() {
        this._subscription = authService.subscribe(() => this.populateState());
        this.populateState();
    }

    componentWillUnmount() {
        authService.unsubscribe(this._subscription);
    }

    populateState = async () => {
        const [isAuthenticated, user] = await Promise.all([authService.isAuthenticated(), authService.getUser()])
        this.setState({
            ...this.state,
            isAuthenticated,
            userName: user && user.name,
            userId: user && Number(user.sub),
        });
        this.fetchItems();
    }

    fetchItems() {
        const params = {
            id: this.props.match.params.id
        };

        DataProvider.getOne("CGames", params).then(object => this.setState({ item: object.data }, () => this.fetchReviews()), () => this.setState({ ...this.state, redirect: true }));

        const genreParams = {
            pagination: {
                ...this.state.pagination,
            },
            sort: {
                field: "id",
                order: "ASC"
            },
            filter: {
                q: undefined,
            }
        }

        DataProvider.getList("GameGenres", genreParams).then(object => { this.setState({ ...this.state, genres: object.data }); });

        const authorParams = {
            pagination: this.state.pagination,
            sort: {
                field: "id",
                order: "ASC"
            },
            filter: {
                q: undefined,
                FriendsOnly: true,
            },
        };

        DataProvider.getList("CAppUsers", authorParams).then(object => this.setState({ ...this.state, authors: object.data }));
    }

    fetchReviews = () => {
        const params = {
            pagination: this.state.reviewPagination,
            sort: {
                field: "id",
                order: "ASC"
            },
            filter: {
                q: undefined,
                ReviewableId: this.state.item.id,
            },
        };
        let pagination = this.state.reviewPagination;
        let hasPrev = pagination.page > 1;
        let startIndex = (pagination.page) * pagination.perPage;
        DataProvider.getList("CReviews", params)
            .then(object => {
                this.setState(
                    {
                        ...this.state,
                        reviewPagination:
                        {
                            ...this.state.reviewPagination,
                            hasPrev: hasPrev,
                            hasNext: startIndex < object.total
                        },
                        reviews: object.data,
                        total: object.total
                    })
            });
    }

    handleSubmitReview = (event) => {
        event.preventDefault();

        const review = this.state.review;

        const params = {
            data: {
                ...review,
                Rating: Number(review.Rating),
                ReviewableId: this.state.item.id,
                UserId: this.state.userId,
            }
        }
        DataProvider.create("CReviews", params).then(() => this.fetchItems());
    }

    handleDiscardReview = (event) => {
        event.preventDefault();

        this.setState({
            ...this.state,
            review: {
                Content: "",
                Rating: 5,
            }
        }, () => this.handleReviewMenu(event));


    }

    handleReviewMenu = (event) => {
        event.preventDefault();

        const writeMode = !this.state.writeMode;
        this.setState(
            {
                ...this.state,
                writeMode: writeMode
            });
    }

    handleReviewDelete = (event) => {
        const params = {
            id: event.target.value,
        }
        DataProvider.delete("CReviews", params).then(object => this.fetchItems());
    }

    handleReviewChange = (event) => {
        event.preventDefault();
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        const name = target.name;

        const review = { ...this.state.review, [name]: value };

        this.setState({
            ...this.state,
            review: review,
        });
    }

    handlePageInc = (event) => {
        event.preventDefault();
        let page = this.state.reviewPagination.page + 1;
        this.setState(
            {
                ...this.state,
                reviewPagination:
                {
                    ...this.state.reviewPagination,
                    page: page
                }
            }, () => this.fetchItems());
    }

    handlePageDec = (event) => {
        event.preventDefault();
        let page = Math.max(this.state.reviewPagination.page - 1, 1);
        this.setState(
            {
                ...this.state,
                reviewPagination:
                {
                    ...this.state.reviewPagination,
                    page: page
                }
            }, () => this.fetchItems());
    }

    handlePageChange = (event) => {
        this.setState(
            {
                ...this.state,
                reviewPagination:
                {
                    ...this.state.reviewPagination,
                    page: event.target.value
                }
            }, () => this.fetchItems());
    }

    handleChange = (event) => {
        event.preventDefault();
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        const name = target.name;

        const item = { ...this.state.item, [name]: value };

        this.setState({
            ...this.state,
            item: item,
        });
    }

    handleSaveChanges = (event) => {
        event.preventDefault();

        const params = {
            data: {
                ...this.state.item,
            },
            id: this.state.item.id,
        }
        DataProvider.update("CGames", params)
            .then(object => this.setState({ item: object.data }));

    }

    handleDiscardChanges = (event) => {
        event.preventDefault();

        this.fetchItems();
    }

    handleEdit = (event) => {
        const editMode = !this.state.editMode;
        this.setState({ ...this.state, editMode: editMode });
    }

    handleDelete = (event) => {
        event.preventDefault();

        const params = {
            id: this.state.item.id,
        }
        DataProvider.delete("CGames", params).then(object => this.fetchItems());
    }

    handleGenreAdd = (event) => {
        event.preventDefault();

        let item = this.state.item;
        let genre = JSON.parse(event.target.value);
        let GenreIds = item.GenreIds;
        let selectedGenres = this.state.selectedGenres;

        if (!GenreIds.includes(genre.id)) {
            GenreIds.push(genre.id);
            selectedGenres.push(genre);
        }

        this.setState({ ...this.state, item: { ...item, GenreIds: GenreIds }, selectedGenres: selectedGenres });
    }

    handleGenrePillClose = (event) => {
        event.preventDefault();

        let item = this.state.item;
        let GenreIds = item.GenreIds;
        let selectedGenres = this.state.selectedGenres;
        GenreIds = GenreIds.filter((value) => value != event.currentTarget.value);
        selectedGenres = selectedGenres.filter((value) => value.id != event.currentTarget.value);
        this.setState({ ...this.state, item: { ...item, GenreIds: GenreIds }, selectedGenres: selectedGenres });
    }

    handleAuthorAdd = (event) => {
        event.preventDefault();

        let item = this.state.item;
        let author = JSON.parse(event.target.value);
        let AuthorIds = item.AuthorIds;
        let selectedAuthors = this.state.selectedAuthors;

        if (!AuthorIds.includes(author.id)) {
            AuthorIds.push(author.id);
            selectedAuthors.push(author);
        }

        this.setState({ ...this.state, item: { ...item, AuthorIds: AuthorIds }, selectedAuthors: selectedAuthors });
    }

    handleAuthorPillClose = (event) => {
        event.preventDefault();

        let item = this.state.item;
        let AuthorIds = item.AuthorIds;
        let selectedAuthors = this.state.selectedAuthors;
        AuthorIds = AuthorIds.filter((value) => value != event.currentTarget.value);
        selectedAuthors = selectedAuthors.filter((value) => value.id != event.currentTarget.value);
        this.setState({ ...this.state, item: { ...item, AuthorIds: AuthorIds }, selectedAuthors: selectedAuthors });
    }

    disown = () => {
        const params = {
            data: { ...this.state.item, IsOwned: false },
            id: this.state.item.id,
        }
        DataProvider.update("CGames", params).then(object => this.setState({ item: object.data }));
    }

    own = () => {
        const params = {
            data: { ...this.state.item, IsOwned: true },
            id: this.state.item.id,
        }
        DataProvider.update("CGames", params).then(object => this.setState({ item: object.data }));
    }

    addButton = <button onClick={this.own} className="btn btn-primary p-3 d-flex flex-grow-1 align-items-center justify-content-center">Add</button>

    removeButton = <button onClick={this.disown} className="btn btn-danger p-3 d-flex flex-grow-1 align-items-center justify-content-center">Remove</button>

    editButton = <button onClick={this.handleEdit} className="btn btn-secondary p-3 d-flex flex-grow-1 align-items-center justify-content-center">Edit</button>

    deleteButton = <button onClick={this.handleDelete} className="btn btn-danger p-3 d-flex flex-grow-1 align-items-center justify-content-center">Delete</button>

    setup = () => {
        this.editMenu =
            <div className="d-flex m-2 align-items-center justify-content-center">
                <div className="card bg-dark col-sm-12 col-md-8 col-lg-6">
                    <div className="card-body px-3 pt-3 pb-0">
                        <form>
                            <div className="form-group">
                                <label for="GameName">Name</label>
                                <input type="text" name="Name" id="GameName" value={this.state.item.Name} onChange={this.handleChange} className="form-control" />
                            </div>
                            <div className="form-group">
                                <label for="GameDescription">Description</label>
                                <textarea name="Description" id="GameDescription" value={this.state.item.Description} onChange={this.handleChange} className="form-control" />
                            </div>
                            <div className="form-group">
                                <label for="GameRules">Rules</label>
                                <textarea name="Rules" id="GameRules" value={this.state.item.Rules} onChange={this.handleChange} className="form-control" />
                            </div>
                            <div className="form-group">
                                <label for="GameMaxPlayers">Max Players</label>
                                <input type="number" min="0" name="MaxPlayers" id="GameMaxPlayers" value={this.state.item.MaxPlayers} onChange={this.handleChange} className="form-control w-25" />
                            </div>
                            <div className="form-group">
                                <label for="GameMinPlayers">Min Players</label>
                                <input type="number" min="0" name="MinPlayers" id="GameMinPlayers" value={this.state.item.MinPlayers} onChange={this.handleChange} className="form-control w-25" />
                            </div>
                            <div className="form-group">
                                <label for="GameImageUrl">Image URL</label>
                                <input type="text" name="ImageUrl" id="GameImageUrl" value={this.state.item.ImageUrl} onChange={this.handleChange} className="form-control" />
                            </div>
                            <div className="form-group w-50">
                                <label for="GameGenres">Add Genre</label>
                                <select name="Type" onChange={this.handleGenreAdd} className="form-control" id="GameGenres">
                                    <option selected value="" disabled >Undefined</option>
                                    {this.state.genres.map((value, index) => {
                                        return <option value={JSON.stringify(value)}>{value.Name}</option>
                                    })}
                                </select>
                                {this.state.selectedGenres.map((value, index) => {
                                    return <FilterPill Name={value.Name} Value={value.id} close={this.handleGenrePillClose} />
                                })}
                            </div>
                            <div className="form-group w-50">
                                <label for="GameAuthors">Add Author</label>
                                <select name="Type" onChange={this.handleAuthorAdd} className="form-control" id="GameAuthors">
                                    <option selected value="" disabled >Undefined</option>
                                    {this.state.authors.map((value, index) => {
                                        return <option value={JSON.stringify(value)}>{value.Username}</option>
                                    })}
                                </select>
                                {this.state.selectedAuthors.map((value, index) => {
                                    return <FilterPill Name={value.Username} Value={value.id} close={this.handleAuthorPillClose} />
                                })}
                            </div>
                            <div className="form-group d-flex">
                                <button className="form-control btn mx-1 btn-success" onClick={this.handleSaveChanges}>Save Changes</button>
                                <button className="form-control btn mx-1 btn-danger" onClick={this.handleDiscardChanges}>Discard Changes</button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>;
        this.writeMenu =
            <div className="d-flex m-3 align-items-center justify-content-center">
                <div className="card bg-dark col-sm-12 col-md-8 col-lg-6 col-xl-5">
                    <div className="card-body px-3 pt-3 pb-0">
                        <form>
                            <div className="form-group">
                                <label for="ReviewRating">Rating</label>
                                <input type="number" min="1" max="5" name="Rating" id="ReviewRating" value={this.state.review.Rating} onChange={this.handleReviewChange} className="form-control w-25" />
                            </div>
                            <div className="form-group">
                                <label for="ReviewContent">Content</label>
                                <textarea placeholder="Writing a review will overwrite your old review if you had one." name="Content" id="ReviewContent" value={this.state.review.Content} onChange={this.handleReviewChange} className="form-control" />
                            </div>
                            <div className="form-group d-flex">
                                <button className="form-control btn mx-1 btn-success" onClick={this.handleSubmitReview}>Submit</button>
                                <button className="form-control btn mx-1 btn-danger" onClick={this.handleDiscardReview}>Discard</button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>;
        if (this.state.item.Authors.length != 0)
            this.Authors = <div>
                <div className="d-flex flex-row align-items-center justify-content-start">
                    <h5 className="card-title align-self-center mr-3">Authors:</h5>
                    {this.state.item.Authors.map((value, index) => {
                        return <h5 className="mr-2">{value}{index < this.state.item.Authors.length - 1 ? "," : null}</h5>
                    })}
                </div>
            </div>;
        else this.Authors = null;
        if (this.state.item.Genres.length != 0)
            this.Genres = <div>
                <div className="d-flex flex-row align-items-center justify-content-start">
                    <h5 className="card-title align-self-center mr-3">Genres:</h5>
                    {this.state.item.Genres.map((value, index) => {
                        return <h5 className="mr-2">{value}{index < this.state.item.Genres.length - 1 ? "," : null}</h5>
                    })}
                </div>
            </div>;
        else this.Genres = null;
        if (this.state.item.Rating != null)
            this.Rating = <div>
                <h5 className="card-title align-self-center">Rating:<span className="px-3">{this.state.item.Rating}</span></h5>
            </div>;
        else this.Rating = <div>
            <h5 className="card-title align-self-center">Rating:<span className="px-3">Unrated</span></h5>
        </div>;

    }

    render() {
        if (this.state.redirect)
            return (<Redirect to="/gamelist/" />)

        if (this.state.item != undefined) {

            this.setup();

            return (
                <div className="d-flex flex-column h-100">
                    <div className="card bg-dark">
                        <img src={this.state.item.ImageUrl} className="card-img-top img-fluid" style={{ height: "10rem", objectFit: "cover" }} alt="..." />
                        <div className="card-body p-0 d-flex justify-content-between">
                            <h1 className="card-title m-3 align-self-center">{this.state.item.Name}</h1>
                            <div className="d-flex w-25 justify-content-end">
                                {this.state.isAuthenticated ? this.state.item.IsOwned ? this.removeButton : this.addButton : null}
                                {this.state.item.IsAuthor ? this.editButton : null}
                                {this.state.item.IsAuthor ? this.deleteButton : null}
                            </div>
                        </div>
                    </div>

                    {this.state.editMode ? this.editMenu : null}

                    <div className="card bg-dark">
                        <div className="card-body px-3 pt-3 pb-0">
                            <div className="">
                                <div className="col-sm-6 col-lg-9 px-0 d-inline-block">
                                    <h5 className="card-title">Description:</h5>
                                    <p className="px-3">{this.state.item.Description}</p>
                                </div>
                                <div className="col-sm-6 col-lg-3 px-0 d-inline-block align-top">
                                    {this.Rating}
                                    <h5 className="card-title">Players:<span className="mr-3" />{this.state.item.MinPlayers} {this.state.item.MinPlayers != this.state.item.MaxPlayers ? " to " + this.state.item.MaxPlayers : null} people</h5>
                                </div>
                            </div>
                            <h5 className="card-title align-self-center">Rules:</h5>
                            <p className="px-3">{this.state.item.Rules}</p>

                            {this.Genres}
                            {this.Authors}
                        </div>
                    </div>

                    <div className="text-center m-5 mt-auto">
                        <h1 className="">Reviews</h1>
                    </div>

                    <nav aria-label="Page navigation">
                        <form>

                            <ul className="pagination justify-content-center">
                                {this.state.isAuthenticated ?
                                    <li className="page-item">
                                        <button onClick={this.handleReviewMenu} className="page-link bg-dark text-light">Write</button>
                                    </li> : null}
                                <li className="page-item">
                                    <button className="page-link" disabled={!this.state.reviewPagination.hasPrev} onClick={this.handlePageDec}>Previous</button>
                                </li>
                                <li className="page-item">
                                    <input type="number" className="page-link text-center" value={this.state.reviewPagination.page} onChange={this.handlePageChange} />
                                </li>
                                <li className="page-item">
                                    <button className="page-link" disabled={!this.state.reviewPagination.hasNext} onClick={this.handlePageInc}>Next</button>
                                </li>

                            </ul>
                        </form>
                    </nav>

                    {this.state.writeMode ? this.writeMenu : null}

                    <div className="pb-1">
                        {this.state.reviews.length == 0 ?
                            <div className="d-flex justify-content-center align-items-center card bg-dark flex-grow-1 p-2">
                                <h3 className="">No reviews to display</h3>
                            </div>
                            :

                            this.state.reviews.map((value, index) => {
                                return <div className="d-flex p-2 m-2 flex-column align-items-start card bg-dark flex-grow-1">
                                    <h4 className="">User: {value.User}</h4>
                                    <h5 className="">Rating: {value.Rating}</h5>
                                    <p className="mt-3 mb-0">{value.Content}</p>
                                    {value.UserId == this.state.userId ? <button className="btn btn-danger align-self-end" value={value.id} onClick={this.handleReviewDelete}>Delete</button> : null}
                                </div>
                            })

                        }
                    </div>


                </div>


            );
        } else
            return (<div>No such game</div>);
    }
}
