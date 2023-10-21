import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import DataProvider from '../admin/DataProvider'

export class PlanCard extends Component {
    static displayName = PlanCard.name;

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
            <div className="col-lg-12 p-2">
                <Link to={"/planlist/" + this.state.item.id}>
                    <div className="card bg-dark border-secondary">

                        <div className="card-body d-flex flex-column justify-content-between">
                            <h2 className="card-title text-truncate">{this.state.item.Name}</h2>
                            <h5>{"Type: " + this.state.item.Type}</h5>
                            <h5>{"Game: "+this.state.item.Game}</h5>
                            <h5>{"Host: " + this.state.item.Host}</h5>
                            <h5>{"Time: " + (new Date(this.state.item.Datetime)).toLocaleString()}</h5>
                            <h5>{"Location: " + this.state.item.Location}</h5>
                        </div>
                    </div>
                </Link>
            </div>
        );
    }
}
