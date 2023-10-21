import { data } from 'jquery';
import React, { Component } from 'react';
import DataProvider from '../admin/DataProvider'
import authService from '../api-authorization/AuthorizeService';
import { Redirect } from 'react-router-dom';

export class Plan extends Component {
    static displayName = Plan.name;

    constructor(props) {
        super(props);
        this.state = {
            item: undefined,
            pagination: {
                page: 1,
                perPage: 50,
                hasPrev: false,
                hasNext: false
            },
            reviewPagination: {
                page: 1,
                perPage: 12,
                hasPrev: false,
                hasNext: false,
            },
            participationRequests: [],
            editMode: false,
            redirect: false,

            reviews: [],
            review: {
                Rating: 5,
                Content: "",
            },
            friends: [],
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

    fetchItems() {
        const params = {
            id: this.props.match.params.id
        };

        DataProvider.getOne("CPlans", params).then(object => this.setState({ item: object.data }, () => this.fetchReviews()), () => this.setState({ ...this.state, redirect: true }));

        const prParams = {
            pagination: this.state.pagination,
            sort: {
                field: "id",
                order: "ASC"
            },
            filter: {
                PlanId: Number(this.props.match.params.id),
            },
        };

        DataProvider.getList("CParticipationRequests", prParams).then(object => this.setState({ ...this.state, participationRequests: object.data }));

        const friendParams = {
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

        DataProvider.getList("CAppUsers", friendParams).then(object => { this.setState({ ...this.state, friends: object.data }); });
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

    componentDidMount() {
        this._subscription = authService.subscribe(() => this.populateState());
        this.populateState();
    }

    componentWillUnmount() {
        authService.unsubscribe(this._subscription);
    }

    handleLeave = (event) => {
        const params = {
            data: {
                ...this.state.item,
                IsPlayer: false,
                IsSpectator: false,
            },
            id: this.state.item.id,
        }
        DataProvider.update("CPlans", params)
            .then(object => this.setState({ item: object.data }));
    }

    handleJoin = (event) => {
        event.preventDefault();

        const params = {
            data: {
                Message: this.state.userName + " has asked to " + event.target.value + " " + this.state.item.Name,
                SenderId: this.state.userId,
                Type: event.target.value,
                PlanId: this.state.item.id,
            },
        }
        DataProvider.create("CParticipationRequests", params)
            .then(object => this.populateState());
    }

    handleInvite = (event) => {
        event.preventDefault();

        const params = {
            data: {
                Message: this.state.userName + " has asked you to " + event.target.value + " " + this.state.item.Name,
                RecipientId: Number(this.state.SelectedUserId),
                Type: event.target.value,
                PlanId: Number(this.state.item.id),
            },
        }
        DataProvider.create("CInvitations", params)
            .then(object => this.populateState());
    }

    handleInviteChange = (event) => {
        event.preventDefault();
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        const name = target.name;

        const state = { ...this.state, [name]: value };

        this.setState({
            ...state
        });
    }

    handleAccept = (event) => {
        event.preventDefault();
        this.commitParticipationRequest(event, true);
    }

    handleDecline = (event) => {
        event.preventDefault();
        this.commitParticipationRequest(event, false);
    }

    commitParticipationRequest = (event, IsAccepted) => {
        const params = {
            data: {
                id: Number(event.target.value),
                IsAccepted: IsAccepted,
            },
            id: event.target.value,
        }
        DataProvider.update("CParticipationRequests", params)
            .then(object => this.populateState());
    }

    handleChange = (event) => {
        event.preventDefault();
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        const name = target.name;

        const plan = { ...this.state.item, [name]: value };

        this.setState({
            ...this.state,
            item: plan,
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
        DataProvider.update("CPlans", params)
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
        const params = {
            id: this.state.item.id,
        }
        DataProvider.delete("CPlans", params).then(object => this.fetchItems());
    }

    editButton = <button onClick={this.handleEdit} className="btn btn-secondary p-3 w-25 d-flex flex-even align-items-center justify-content-center">Edit</button>
    deleteButton = <button onClick={this.handleDelete} className="btn btn-danger p-3 w-25 d-flex flex-even align-items-center justify-content-center">Delete</button>
    leaveButton = null;
    playButton = null;
    spectateButton = null;

    setup = () => {
        if (this.state.item.IsSpectator || this.state.item.IsPlayer) {
            this.leaveButton = <button onClick={this.handleLeave} className="btn btn-primary p-3 w-25 d-flex flex-even align-items-center justify-content-center">Leave</button>
            this.playButton = null;
            this.spectateButton = null;
        }else {
            this.leaveButton = null;
            this.playButton =
                <button onClick={this.handleJoin} value="play" className="btn btn-primary p-3 w-25 d-flex flex-even align-items-center justify-content-center">Play</button>;
            this.spectateButton =
                <button onClick={this.handleJoin} value="spectate" className="btn btn-info p-3 w-25 d-flex flex-even align-items-center justify-content-center">Spectate</button>;
        }
        this.editMenu =
            <div className="d-flex m-2 align-items-center justify-content-center">
                <div className="card bg-dark col-sm-12 col-md-8 col-lg-6">
                    <div className="card-body px-3 pt-3 pb-0">
                        <form>
                            <div className="form-group">
                                <label for="PlanName">Name</label>
                                <input type="text" name="Name" onChange={this.handleChange} className="form-control" id="PlanName" value={this.state.item.Name} />
                            </div>
                            <div className="form-group">
                                <label for="PlanLocation">Location</label>
                                <input type="text" name="Location" onChange={this.handleChange} className="form-control" id="PlanLocation" value={this.state.item.Location} />
                            </div>
                            <div className="form-group">
                                <label for="PlanTime">Time</label>
                                <input type="datetime-local" name="Datetime" onChange={this.handleChange} className="form-control w-50" id="PlanTime" value={this.state.item.Datetime} />
                            </div>
                            <div className="form-group d-flex">
                                <button className="form-control btn mx-1 btn-success" onClick={this.handleSaveChanges}>Save Changes</button>
                                <button className="form-control btn mx-1 btn-danger" onClick={this.handleDiscardChanges}>Discard Changes</button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>;

        this.requestsPanel =
            <div>
                <h5 className="card-title align-self-center">Requests:</h5>
                <div className="d-flex flex-column w-70">
                    {this.state.participationRequests.map((value, index) => {
                        return <div className="d-flex justify-content-between align-items-center my-1"><p className="px-3 py-0 my-1">{value.Message}</p><div><button className="btn btn-primary" onClick={this.handleAccept} value={value.id}>Accept</button><button className="btn btn-danger" onClick={this.handleDecline} value={value.id}>Decline</button></div></div>

                    })}
                </div>
            </div>;

        this.inviteMenu =
            <div className="d-flex m-3 align-items-center justify-content-center">
                <div className="card bg-dark col-sm-12 col-md-8 col-lg-6 col-xl-5">
                    <div className="card-body px-3 pt-3 pb-0">
                        <form>
                            <div className="form-group">
                                <label for="ReviewRating">Select User</label>
                                <select name="SelectedUserId" onChange={this.handleInviteChange} className="form-control w-50" id="GameAuthors">
                                    <option selected value="" disabled >Undefined</option>
                                    {this.state.friends.map((value, index) => {
                                        return <option value={value.id}>{value.Username}</option>
                                    })}
                                </select>
                            </div>
                            <div className="form-group d-flex">
                                <button className="form-control btn mx-1 btn-primary" value="play" onClick={this.handleInvite}>Invite To Play</button>
                                <button className="form-control btn mx-1 btn-info" value="spectate" onClick={this.handleInvite}>Invite To Spectate</button>
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

        if (this.state.item.Rating != null)
            this.Rating = <div>
                <h5 className="card-title align-self-center">Rating:<span className="px-3">{this.state.item.Rating}</span></h5>
            </div>;
        else this.Rating = null;
    }


    handleKickPlayer = (event) => {
        event.preventDefault();

        const plan = this.state.item;
        let Players = plan.Players
        let Player = event.target.value;

        Players = Players.filter((value) => value != Player);
        this.setState({ ...this.state, item: { ...plan, Players: Players } }, () => this.handleSaveChanges(event));


    }

    handleKickSpectator = (event) => {
        event.preventDefault();

        const plan = this.state.item;
        let Spectators = plan.Spectators
        let Spectator = event.target.value;

        Spectators = Spectators.filter((value) => value != Spectator);
        this.setState({ ...this.state, item: { ...plan, Spectators: Spectators } }, () => this.handleSaveChanges(event));
    }

    render() {
        if (this.state.redirect)
            return (<Redirect to="/planlist/" />)

        if (this.state.item != undefined) {

            this.setup();

            return (
                <div className="d-flex flex-column h-100">
                    <div className="card bg-dark">
                        <div className="card-body p-0 d-flex justify-content-between">
                            <div className="d-flex flex-column justify-content-start align-items-start">
                                <h1 className="card-title m-3">{this.state.item.Name}</h1>
                            </div>
                            <div className="d-flex justify-content-end align-items-stretch col-sm-6 col-md-5 col-lg-4 px-0">
                                {this.leaveButton}
                                {this.playButton}
                                {this.spectateButton}
                                {this.state.item.IsHost ? this.editButton : null}
                                {this.state.item.IsHost ? this.deleteButton : null}
                            </div>
                        </div>
                    </div>

                    {this.state.editMode ? this.editMenu : null}
                    {this.state.item.IsHost ? this.inviteMenu : null}

                    <div className="card bg-dark">
                        <div className="card-body px-3 pt-3 pb-0">
                            <div className="">
                                <div className="w-50 d-inline-block">
                                    <h5 className="card-title align-self-center">Game:<span className="px-3">{this.state.item.Game}</span></h5>
                                    <h5 className="card-title align-self-center">Host:<span className="px-3">{this.state.item.Host}</span></h5>
                                    <h5 className="card-title align-self-center">Type:<span className="px-3">{this.state.item.Type}</span></h5>
                                </div>
                                <div className="w-50 d-inline-block align-top">
                                    <h5 className="card-title align-self-center">Time: <span className="px-3">    {(new Date(this.state.item.Datetime)).toLocaleString()}</span></h5>
                                    <h5 className="card-title align-self-center">Location: <span className="px-3">    {this.state.item.Location}</span></h5>
                                    {this.Rating}
                                </div>
                            </div>
                            
                            <div className="">
                                <div className="w-50 d-inline-block">
                                    <h5 className="card-title align-self-center">Players (Max {this.state.item.MaxPlayers}):</h5>
                                    {this.state.item.Players.map((value, index) => {
                                        return <div className="d-flex align-items-center pl-3 my-1"><p className="p-0 my-0 mx-2">{value}</p><button onClick={this.handleKickPlayer} value={value} className="btn btn-danger">Kick</button></div>
                                    })}
                                </div>
                                <div className="w-50 d-inline-block align-top">
                                    <h5 className="card-title align-self-center">Spectators (Max {this.state.item.MaxSpectators}):</h5>
                                    {this.state.item.Spectators.map((value, index) => {
                                        return <div className="d-flex align-items-center pl-3 my-1"><p className="p-0 my-0 mx-2">{value}</p><button onClick={this.handleKickSpectator} value={value} className="btn btn-danger">Kick</button></div>
                                    })}
                                </div>
                            </div>
                            
                            
                            
                            {this.requestsPanel}
                        </div>
                    </div>

                    <div className="text-center m-5 mt-auto">
                        <h1 className="">Reviews</h1>
                    </div>

                    <nav aria-label="Page navigation">
                        <form>

                            <ul className="pagination justify-content-center">
                                <li className="page-item">
                                    <button onClick={this.handleReviewMenu} className="page-link bg-dark text-light">Write</button>
                                </li>
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
                            <div className="d-flex justify-content-center align-items-center card bg-dark flex-grow-1">
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
            return (<div>No such plan</div>);
    }
}
