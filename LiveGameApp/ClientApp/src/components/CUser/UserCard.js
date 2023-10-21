import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import DataProvider from '../admin/DataProvider'

export class UserCard extends Component {
    static displayName = UserCard.name;

    constructor(props) {
        super(props);
        this.state = {
            item: props.item
        };
    }

    componentWillReceiveProps(nextProps) {
        this.setState({ item: nextProps.item });
    }

    unfriend = (event) => {
        event.preventDefault();

        let senderId = Number(this.props.senderId);
        let recipientId = Number(this.state.item.id);

        let id1 = senderId > recipientId ? recipientId : senderId;
        let id2 = senderId > recipientId ? senderId : recipientId;

        const params = {
            id: id1 + "-" + id2,
        }
        DataProvider.delete("CFriends", params)
            .then(object => this.props.refresh());
    }

    befriend = (event) => {
        event.preventDefault();

        const params = {
            data: {
                Message: this.props.senderName + " wants to be your friend.",
                RecipientId: Number(this.state.item.id),
                SenderId: Number(this.props.senderId),
            },
        }
        DataProvider.create("CFriendRequests", params)
            .then(object => this.props.refresh());
    }

    render() {
        if (this.props.isFriend) {
            this.actionButton = <button onClick={this.unfriend} className="btn btn-danger">Unfriend</button>
        } else {
            this.actionButton = <button onClick={this.befriend} className="btn btn-info">Befriend</button>
        }

        if (this.props.noAction)
            return (
                <div className="col-lg-12 p-0 m-0">
                    <div className="card bg-dark">
                            <div className="card-body p-2 d-flex justify-content-between">
                                <h5 className="card-title m-0 pt-1 text-truncate">{this.state.item.Username}</h5>
                            </div>
                        </div>
                </div>
            );
        else
            return (
                <div className="col-lg-12 p-0 m-0">
                    <Link to={"/userlist/" + this.state.item.id}>
                        <div className="card bg-dark">

                            <div className="card-body p-2 d-flex justify-content-between">
                                <h5 className="card-title m-0 pt-1 text-truncate">{this.state.item.Username}</h5>
                                {this.actionButton}
                            </div>
                        </div>
                    </Link>
                </div>
            );
    }
}
