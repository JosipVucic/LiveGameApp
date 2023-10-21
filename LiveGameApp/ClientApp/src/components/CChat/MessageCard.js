import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import DataProvider from '../admin/DataProvider'

export class MessageCard extends Component {
    static displayName = MessageCard.name;

    constructor(props) {
        super(props);
        this.state = {
            item: props.item
        };
    }

    componentWillReceiveProps(nextProps) {
        this.setState({ item: nextProps.item });
    }

    render() {
        return (
            <div className={(this.props.userId == this.state.item.SenderId ? "align-self-end" : "align-self-start") + " w-50 p-2"}>
                {this.props.userId == this.state.item.SenderId ?
                    <div className="card bg-primary talk-bubble tri-right round border border-primary right-top tri-color-primary">
                        <div className="card-body d-flex justify-content-between">
                            <h5 className="card-title">{this.state.item.Content}</h5>
                        </div>
                    </div>
                    :
                    <div className="card bg-secondary talk-bubble tri-left round border border-secondary left-top tri-color-info">
                        <div className="card-body d-flex justify-content-between">
                            <h5 className="card-title">{this.state.item.Content}</h5>
                        </div>
                    </div>
                    }
            </div>
        );
    }
}
