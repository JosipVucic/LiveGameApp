import React, { Component } from 'react';
import DataProvider from '../admin/DataProvider';
import { Redirect } from 'react-router-dom';
import authService from '../api-authorization/AuthorizeService';

export class PlanCreate extends Component {
    static displayName = PlanCreate.name;

    constructor(props) {
        super(props);
        this.state = {
            plan: {
                Name: "",
                Datetime: new Date(),
                Location: "",
                MaxPlayers: 0,
                MaxSpectators: 0,
                Type: "Tournament",
                PrivacyType: "Public",
            },
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
    }

    componentDidMount() {
        this._subscription = authService.subscribe(() => this.populateState());
        this.populateState();
    }

    componentWillUnmount() {
        authService.unsubscribe(this._subscription);
    }

    handleChange = (event) => {
        event.preventDefault();

        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        const name = target.name;

        const plan = { ...this.state.plan, [name]: value };

        this.setState({
            ...this.state,
            plan: plan,
        });
    }

    handleCreate = (event) => {
        event.preventDefault();

        const plan = this.state.plan;

        const params = {
            data: {
                ...plan,
                MaxPlayers: Number(plan.MaxPlayers),
                MaxSpectators: Number(plan.MaxSpectators),
                HostUserId: Number(this.state.userId),
                GameId: Number(this.props.match.params.id),
            },
        }
        DataProvider.create("CPlans", params)
            .then(object => this.setState({ id: object.data.id }));

    }

    handleCancel = (event) => {
        event.preventDefault();

        this.setState({
            ...this.state,
            plan: {
                Name: "",
                Datetime: new Date(),
                Location: "",
                MaxPlayers: 0,
                MaxSpectators: 0,
                Type: "Tournament",
                PrivacyType: "Public",
            }
        });
    }

    render() {
        return (
            <div>
                <div className="text-center m-3">
                    <h1 className="display-4">Create plan</h1>
                </div>
                <div className="d-flex align-items-center justify-content-center">
                    <div className="card bg-dark col-sm-12 col-md-8 col-lg-6">
                    <div className="card-body px-3 pt-3 pb-0">
                        <form>
                            <div className="form-group">
                                <label for="PlanName">Name</label>
                                <input type="text" name="Name" onChange={this.handleChange} className="form-control" id="PlanName" value={this.state.plan.Name} />
                            </div>
                            <div className="form-group">
                                <label for="PlanLocation">Location</label>
                                <input type="text" name="Location" onChange={this.handleChange} className="form-control" id="PlanLocation" value={this.state.plan.Location} />
                            </div>
                            <div className="form-group">
                                <label for="PlanTime">Time</label>
                                <input type="datetime-local" name="Datetime" onChange={this.handleChange} className="form-control w-50" id="PlanTime" value={this.state.plan.Datetime} />
                            </div>
                            <div className="form-group">
                                <label for="PlanMaxPlayers">Max Players</label>
                                <input type="number" min="0" name="MaxPlayers" onChange={this.handleChange} className="form-control w-25" id="PlanMaxPlayers" value={this.state.plan.MaxPlayers} />
                            </div>
                            <div className="form-group">
                                <label for="PlanMaxSpectators">Max Spectators</label>
                                <input type="number" min="0" name="MaxSpectators" onChange={this.handleChange} className="form-control w-25" id="PlanMaxSpectators" value={this.state.plan.MaxSpectators} />
                            </div>
                            <div className="form-group">
                                <label for="PlanType">Type</label>
                                <select name="Type" onChange={this.handleChange} className="form-control w-50" id="PlanType">
                                    <option selected={this.state.plan.Type == "Game"} value="Game">Game</option>
                                    <option selected={this.state.plan.Type == "Tournament"} value="Tournament">Tournament</option>
                                </select>
                            </div>
                            <div className="form-group">
                                <label for="PlanPrivacyType">Privacy type</label>
                                <select name="PrivacyType" onChange={this.handleChange} className="form-control w-50" id="PlanPrivacyType">
                                    <option selected={this.state.plan.PrivacyType == "Private"} value="Private">Private</option>
                                    <option selected={this.state.plan.PrivacyType == "Public"} value="Public">Public</option>
                                </select>
                            </div>
                            <div className="form-group d-flex">
                                <button className="form-control btn mx-1 btn-success" onClick={this.handleCreate}>Create</button>
                                <button className="form-control btn mx-1 btn-danger" onClick={this.handleCancel}>Cancel</button>
                            </div>
                        </form>
                    </div>
                </div>
                </div>
                {this.state.id != 0 ? <Redirect to={"/planlist/" + this.state.id} /> : null}
            </div>
        );
    }
}
