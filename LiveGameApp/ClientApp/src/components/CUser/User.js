import { data } from 'jquery';
import React, { Component } from 'react';
import DataProvider from '../admin/DataProvider'
import authService from '../api-authorization/AuthorizeService';
import { GameAlbum } from '../CGame/GameAlbum';

export class User extends Component {
    static displayName = User.name;

    constructor(props) {
        super(props);
        this.state = {
            item: undefined,
            pagination: {
                page: 1,
                perPage: 50,
                pages: 1,
                hasPrev: false,
                hasNext: false
            },
            games: [],
            invitations: [],
            friendRequests: [],
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

        DataProvider.getOne("CAppUsers", params).then(object => { this.setState({ item: object.data }); });

        const gameParams = {
            pagination: this.state.pagination,
            sort: {
                field: "id",
                order: "ASC"
            },
            filter: {
                OwnerIds: [Number(this.props.match.params.id)],
            },
        };

        DataProvider.getList("CGames", gameParams).then(object => { this.setState({ ...this.state, games: object.data }); });

        const inviteParams = {
            pagination: this.state.pagination,
            sort: {
                field: "id",
                order: "ASC"
            },
        };

        DataProvider.getList("CInvitations", inviteParams).then(object => this.setState({ ...this.state, invitations: object.data }));

        const friendRequestParams = {
            pagination: this.state.pagination,
            sort: {
                field: "id",
                order: "ASC"
            },
        };

        DataProvider.getList("CFriendRequests", friendRequestParams).then(object => this.setState({ ...this.state, friendRequests: object.data }));
    }

    componentDidMount() {
        this._subscription = authService.subscribe(() => this.populateState());
        this.populateState();
    }

    componentWillUnmount() {
        authService.unsubscribe(this._subscription);
    }

    handleAcceptInvite = (event) => {
        event.preventDefault();
        this.commitInvite(event, true);
    }

    handleAcceptFriendRequest = (event) => {
        event.preventDefault();
        this.commitFriendRequest(event, true);
    }

    handleDeclineInvite = (event) => {
        event.preventDefault();
        this.commitInvite(event, false);
    }

    handleDeclineFriendRequest = (event) => {
        event.preventDefault();
        this.commitFriendRequest(event, false);
    }

    commitInvite = (event, IsAccepted) => {
        const params = {
            data: {
                id: Number(event.target.value),
                IsAccepted: IsAccepted,
            },
            id: event.target.value,
        }
        DataProvider.update("CInvitations", params)
            .then(object => this.populateState());
    }

    commitFriendRequest = (event, IsAccepted) => {
        const params = {
            data: {
                id: Number(event.target.value),
                IsAccepted: IsAccepted,
            },
            id: event.target.value,
        }
        DataProvider.update("CFriendRequests", params)
            .then(object => this.populateState());
    }

    render() {
        if (this.state.item != undefined) {

            if (this.state.item.id == this.state.userId)
                this.authorized = true;

            return (
                <div className="d-flex flex-column h-100">
                    <div className="card bg-dark">
                        <div className="card-body p-0 d-flex flex-column justify-content-between">
                            <h1 className="card-title mt-3 align-self-center">{this.state.item.Username}</h1>

                        </div>
                    </div>

                    <div className="card bg-dark flex-grow-1">
                        <div className="card-body px-3 pt-3 pb-0">
                            <div className="">
                                <div className="col-sm-6 px-0 d-inline-block">
                                    <h5 className="card-title align-self-center">Email:<span className="px-3">{this.state.item.Email == null ? "Private" : this.state.item.Email}</span></h5>
                                    {this.authorized ?
                                        <div>
                                            <h5 className="card-title align-self-center">Friend requests:</h5>
                                            <div className="d-flex flex-column w-70">
                                                {this.state.friendRequests.map((value, index) => {
                                                    return <div className="d-flex flex-column justify-content-between align-items-start"><p className="px-3 py-0 my-1">{value.Message}</p><div className="align-self-end mr-5"><button className="btn btn-primary" onClick={this.handleAcceptFriendRequest} value={value.id}>Accept</button><button className="btn btn-danger" onClick={this.handleDeclineFriendRequest} value={value.id}>Decline</button></div></div>
                                                })}
                                            </div>
                                        </div> : null}
                                </div>
                                <div className="col-sm-6 px-0 d-inline-block align-top">
                                    <h5 className="card-title align-self-center">Date of birth:<span className="px-3">    {this.state.item.DateOfBirth == null ? "Private" : (new Date(this.state.item.DateOfBirth)).toLocaleString([], { year: 'numeric', month: '2-digit', day: '2-digit' })}</span></h5>
                                    {this.authorized ?
                                        <div>
                                            <h5 className="card-title align-self-center">Invitations:</h5>
                                            <div className="d-flex flex-column w-70">
                                                {this.state.invitations.map((value, index) => {
                                                    return <div className="d-flex flex-column justify-content-between align-items-start"><p className="px-3 py-0 my-1">{value.Message}</p><div className="align-self-end mr-5"><button className="btn btn-primary" onClick={this.handleAcceptInvite} value={value.id}>Accept</button><button className="btn btn-danger" onClick={this.handleDeclineInvite} value={value.id}>Decline</button></div></div>
                                                })}
                                            </div>
                                        </div> : null}
                                </div>
                            </div>

                            
                            
                            <h5 className="card-title align-self-center">Owned games:</h5>
                            <GameAlbum games={this.state.games} />
                        </div>


                    </div>
                </div>
            );

        } else
            return (<div>No such user</div>);
    }
}
