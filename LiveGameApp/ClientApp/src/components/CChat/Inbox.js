import React, { Component } from 'react';
import DataProvider from '../admin/DataProvider'
import authService from '../api-authorization/AuthorizeService';
import { MessageCard } from './MessageCard';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { UserCard } from '../CUser/UserCard';

export class Inbox extends Component {
    static displayName = Inbox.name;

    constructor(props) {
        super(props);
        this.state = {
            pagination: {
                page: 1,
                perPage: 50,
                pages: 1,
                hasPrev: false,
                hasNext: false
            },
            friends: [],
            messages: [],
            message: "",
            SelectedUserId: 0,
            connection: null,
        };
    }

    populateState = async () => {
        const [isAuthenticated, user] = await Promise.all([authService.isAuthenticated(), authService.getUser()])
        const token = await authService.getAccessToken();

        const newConnection = new HubConnectionBuilder()
            .withUrl('https://localhost:5001/hubs/chat', { accessTokenFactory: () => token })
            .withAutomaticReconnect()
            .build();

        this.setState({
            ...this.state,
            isAuthenticated,
            userName: user && user.name,
            userId: user && Number(user.sub),
            connection: newConnection,
        }, () => this.setupConnection());
        this.fetchItems();


    }

    setupConnection = () => {
        let connection = this.state.connection;
        if (connection) {
            connection.start()
                .then(result => {
                    console.log('Connected!');

                    connection.on('ReceiveMessage', message => {
                        const updatedMessages = this.state.messages;
                        const newMessage = {
                            id: message.id,
                            Content: message.content,
                            SenderId: message.senderId,
                            RecipientId: message.recipientId,
                            Datetime: message.datetime
                        }
                        updatedMessages.push(newMessage);

                        this.setState({ ...this.state, messages: updatedMessages });
                    });
                })
                .catch(e => console.log('Connection failed: ', e));
        }
    }

    fetchItems() {
        const friendParams = {
            pagination: this.state.pagination,
            sort: {
                field: "id",
                order: "ASC"
            },
            filter: {
                q: this.state.search,
                FriendsOnly: true,
            },
        };

        DataProvider.getList("CAppUsers", friendParams).then(object => { this.setState({ ...this.state, friends: object.data }); });

        const params = {
            pagination: this.state.pagination,
            sort: {
                field: "id",
                order: "ASC"
            },
            filter: {
                SenderId: Number(this.state.SelectedUserId),
                RecipientId: Number(this.state.SelectedUserId),
            },
        };

        DataProvider.getList("CDirectMessages", params).then(object => { this.setState({ ...this.state, messages: object.data }); });
    }

    handleChange = (event) => {
        event.preventDefault();
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        const name = target.name;

        const state = { ...this.state, [name]: value };

        this.setState({
            ...state
        });
    }

    handleRealtimeChange = (event) => {
        event.preventDefault();
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        const name = target.name;

        const state = { ...this.state, [name]: value };

        this.setState({
            ...state
        }, () => this.fetchItems());
    }

    handleInboxChange = (event) => {
        event.preventDefault();
        const target = event.currentTarget;
        const value = JSON.parse(target.value);

        const state = { ...this.state, SelectedUserId: value.id, SelectedUserName: value.Username };

        this.setState({
            ...state
        }, () => this.fetchItems());
    }

    handleSend = (event) => {
        event.preventDefault();

        if (this.state.SelectedUserId == 0) {
            alert("Please select the message recipient.")
            return;
        }
        if (this.state.userId == 0) {
            alert("You are not authorized to send messages. Please log in.")
            return;
        }
        if (this.state.message.trim() == "") {
            alert("Cannot send empty messages")
            return;
        }

        this.sendMessage();
    }

    sendMessage = async () => {
        const chatMessage = {
            SenderId: Number(this.state.userId),
            RecipientId: Number(this.state.SelectedUserId),
            content: this.state.message
        };

        if (this.state.connection.connectionStarted) {
            try {
                await this.state.connection.send('SendMessage', chatMessage);
                const params = {
                    data: {
                        ...chatMessage
                    },
                }
                DataProvider.create("CDirectMessages", params)
                    .then(object => this.setState({ id: object.data.id }));
            }
            catch (e) {
                console.log(e);
            }
        }
        else {
            alert('No connection to server yet.');
        }


    }

    componentDidMount() {
        this._subscription = authService.subscribe(() => this.populateState());
        this.populateState();
    }

    componentWillUnmount() {
        authService.unsubscribe(this._subscription);
    }

    render() {

        return (
            <div className="col-lg-10 col-xl-8 mx-auto h-100">
                <div className="d-flex flex-column align-items-stretch h-100">

                    <div className="card bg-dark">
                        <div className="card-body p-0 d-flex flex-column justify-content-between align-items-center">
                            <h1 className="card-title mt-3 align-self-center">Inbox</h1>
                        </div>
                    </div>
                    <div className="d-flex">
                        <div className="d-flex flex-column align-items-center justify-content-start w-25">
                            <div className="card bg-dark w-100">
                                <form className="p-2 w-100">
                                    <input type="text" className="form-control" name="search" value={this.state.search} placeholder="Search Username" onChange={this.handleRealtimeChange} />
                                </form>
                            </div>
                            {this.state.friends.map((value, index) => {
                                if (value.id == this.state.userId) return null;
                                return <button name="SelectedUserId" onClick={this.handleInboxChange} value={JSON.stringify(value)} className="btn text-light p-0 m-0 w-100"><UserCard noAction item={value} /></button>
                            })}
                        </div>
                        {this.state.SelectedUserId != 0 ?
                            <div className="flex-grow-1">
                                <div className="card bg-dark">
                                    <div className="d-flex justify-content-center align-self-stretch">
                                        <h3 className="card-title mt-3 align-self-center">{this.state.SelectedUserName}</h3>
                                    </div>
                                </div>
                                <div className="card bg-dark">
                                    <div className="card-body px-3 pt-3 pb-0 d-flex flex-column justify-content-between align-items-center">
                                        <div className="d-flex flex-column align-self-stretch">
                                            {this.state.messages.map((value, index) => {
                                                return <MessageCard item={value} userId={this.state.userId} />
                                            })}
                                        </div>
                                        <form className="p-2 d-flex align-items-center w-100">
                                            <div className="form-group flex-grow-1">
                                                <input type="text" name="message" id="DirectMessage" value={this.state.message} onChange={this.handleChange} className="form-control" />
                                            </div>
                                            <div className="form-group">
                                                <button className="form-control btn btn-info" onClick={this.handleSend}>Send</button>
                                            </div>
                                        </form>
                                    </div>
                                </div>
                            </div> : null}
                    </div>
                </div>
            </div>
        );

    }
}
