import React, { Component } from 'react';
import DataProvider from '../admin/DataProvider';
import { Redirect } from 'react-router-dom';
import { FilterPill } from '../CUtil/FilterPill';
import authService from '../api-authorization/AuthorizeService';

export class GameCreate extends Component {
    static displayName = GameCreate.name;

    constructor(props) {
        super(props);
        this.state = {
            game: {
                Name: "",
                MaxPlayers: 0,
                MinPlayers: 0,
                ImageUrl: "",
                Description: "",
                Rules: "",
                GenreIds: [],
                AuthorIds: [],
                IsAuthor: true,
            },
            pagination: {
                page: 1,
                perPage: 50,
                pages: 1,
                hasPrev: false,
                hasNext: false
            },
            genres: [],
            selectedGenres: [],
            authors: [],
            selectedAuthors: [],
            id: 0,
            isAuthenticated: false,
            userName: null,
            userId: null,
        };
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

    componentDidMount() {
        this._subscription = authService.subscribe(() => this.populateState());
        this.populateState();
    }

    componentWillUnmount() {
        authService.unsubscribe(this._subscription);
    }

    fetchItems() {
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

        DataProvider.getList("CAppUsers", authorParams).then(object => { this.setState({ ...this.state, authors: object.data }); });
    }

    handleChange = (event) => {
        event.preventDefault();

        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        const name = target.name;

        const game = { ...this.state.game, [name]: value };

        this.setState({
            ...this.state,
            game: game,
        });
    }

    handleCreate = (event) => {
        event.preventDefault();

        const game = this.state.game;

        if (game.Name == "" || game.Description == "" || game.Rules == "" || game.ImageUrl == "" || game.MaxPlayers == 0 || game.MinPlayers == 0) {
            alert("Please fill all the fields");
            return;
        }

        game.AuthorIds.push(Number(this.state.userId))

        const params = {
            data: {
                ...game,
                MaxPlayers: Number(game.MaxPlayers),
                MinPlayers: Number(game.MinPlayers)
            },
        }
        DataProvider.create("CGames", params)
            .then(object => this.setState({ id: object.data.id }));

    }

    handleCancel = (event) => {
        event.preventDefault();

        this.setState({
            ...this.state, game: {
                Name: "",
                MaxPlayers: 0,
                MinPlayers: 0,
                ImageUrl: "",
                Description: "",
                Rules: "",
            }
        });
    }

    handleGenreAdd = (event) => {
        event.preventDefault();

        let game = this.state.game;
        let genre = JSON.parse(event.target.value);
        let GenreIds = game.GenreIds;
        let selectedGenres = this.state.selectedGenres;

        if (!GenreIds.includes(genre.id)) {
            GenreIds.push(genre.id);
            selectedGenres.push(genre);
        }

        this.setState({ ...this.state, game: { ...game, GenreIds: GenreIds }, selectedGenres: selectedGenres });
    }

    handleGenrePillClose = (event) => {
        event.preventDefault();

        let game = this.state.game;
        let GenreIds = game.GenreIds;
        let selectedGenres = this.state.selectedGenres;
        GenreIds = GenreIds.filter((value) => value != event.currentTarget.value);
        selectedGenres = selectedGenres.filter((value) => value.id != event.currentTarget.value);
        this.setState({ ...this.state, game: { ...game, GenreIds: GenreIds }, selectedGenres: selectedGenres });
    }

    handleAuthorAdd = (event) => {
        event.preventDefault();

        let game = this.state.game;
        let author = JSON.parse(event.target.value);
        let AuthorIds = game.AuthorIds;
        let selectedAuthors = this.state.selectedAuthors;

        if (!AuthorIds.includes(author.id)) {
            AuthorIds.push(author.id);
            selectedAuthors.push(author);
        }

        this.setState({ ...this.state, game: { ...game, AuthorIds: AuthorIds }, selectedAuthors: selectedAuthors });
    }

    handleAuthorPillClose = (event) => {
        event.preventDefault();

        let game = this.state.game;
        let AuthorIds = game.AuthorIds;
        let selectedAuthors = this.state.selectedAuthors;
        AuthorIds = AuthorIds.filter((value) => value != event.currentTarget.value);
        selectedAuthors = selectedAuthors.filter((value) => value.id != event.currentTarget.value);
        this.setState({ ...this.state, game: { ...game, AuthorIds: AuthorIds }, selectedAuthors: selectedAuthors });
    }

    render() {
        return (
            <div>
                <div className="text-center m-3">
                    <h1 className="display-4">Create game</h1>
                </div>
                <div className="d-flex align-items-center justify-content-center">
                    <div className="card bg-dark col-sm-12 col-md-8 col-lg-6">
                        <div className="card-body px-3 pt-3 pb-0">
                            <form>
                                <div className="form-group">
                                    <label for="GameName">Name</label>
                                    <input type="text" name="Name" id="GameName" value={this.state.game.Name} onChange={this.handleChange} className="form-control" />
                                </div>
                                <div className="form-group">
                                    <label for="GameDescription">Description</label>
                                    <textarea name="Description" id="GameDescription" value={this.state.game.Description} onChange={this.handleChange} className="form-control" />
                                </div>
                                <div className="form-group">
                                    <label for="GameRules">Rules</label>
                                    <textarea name="Rules" id="GameRules" value={this.state.game.Rules} onChange={this.handleChange} className="form-control" />
                                </div>
                                <div className="form-group">
                                    <label for="GameMaxPlayers">Max Players</label>
                                    <input type="number" min="0" name="MaxPlayers" id="GameMaxPlayers" value={this.state.game.MaxPlayers} onChange={this.handleChange} className="form-control w-25" />
                                </div>
                                <div className="form-group">
                                    <label for="GameMinPlayers">Min Players</label>
                                    <input type="number" min="0" name="MinPlayers" id="GameMinPlayers" value={this.state.game.MinPlayers} onChange={this.handleChange} className="form-control w-25" />
                                </div>
                                <div className="form-group">
                                    <label for="GameImageUrl">Image URL</label>
                                    <input type="text" name="ImageUrl" id="GameImageUrl" value={this.state.game.ImageUrl} onChange={this.handleChange} className="form-control" />
                                </div>
                                <div className="form-group">
                                    <label for="GameGenres">Add Genre</label>
                                    <select name="Type" onChange={this.handleGenreAdd} className="form-control w-50" id="GameGenres">
                                        <option selected value="" disabled >Undefined</option>
                                        {this.state.genres.map((value, index) => {
                                            return <option value={JSON.stringify(value)}>{value.Name}</option>
                                        })}
                                    </select>
                                    {this.state.selectedGenres.map((value, index) => {
                                        return <FilterPill Name={value.Name} Value={value.id} close={this.handleGenrePillClose} />
                                    })}
                                </div>
                                <div className="form-group">
                                    <label for="GameAuthors">Add Author</label>
                                    <select name="Type" onChange={this.handleAuthorAdd} className="form-control w-50" id="GameAuthors">
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
                                    <button className="form-control btn mx-1 btn-success" onClick={this.handleCreate}>Create</button>
                                    <button className="form-control btn mx-1 btn-danger" onClick={this.handleCancel}>Reset</button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
                {this.state.id != 0 ? <Redirect to={"/gamelist/" + this.state.id} /> : null}
            </div>
        );
    }
}
