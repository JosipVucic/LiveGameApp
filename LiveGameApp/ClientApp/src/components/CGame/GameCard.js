import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import DataProvider from '../admin/DataProvider'

export class GameCard extends Component {
    static displayName = GameCard.name;

    constructor(props) {
        super(props);
        this.state = {
            game: props.game
        };
    }

    componentWillReceiveProps(nextProps) {
        this.setState({ game: nextProps.game });
    }

    disown = () => {
        const params = {
            data: { ...this.state.game, IsOwned: false },
            id: this.state.game.id,
        }
        DataProvider.update("CGames", params).then(object => { this.setState({ game: object.data }); this.props.click(); });
    }
    
    own = () => {
        const params = {
            data: {...this.state.game, IsOwned: true},
            id: this.state.game.id,
        }
        DataProvider.update("CGames", params).then(object => { this.setState({ game: object.data }); this.props.click(); });
    }

    plan = () => {
        this.props.plan(this.state.game.id);
    }

    addButton = <button onClick={this.own} className="btn btn-primary p-3 d-flex align-items-center">{this.props.AddBtnName}</button>
    
    removeButton = <button onClick={this.disown} className="btn btn-danger p-3 d-flex align-items-center">{this.props.RemoveBtnName}</button>

    planButton = <button onClick={this.plan} className="btn btn-info p-3 d-flex align-items-center">{this.props.PlanBtnName}</button>


    render() {
        return (
            <div className="col-md-6 col-lg-3 p-2">
                <div className="card bg-dark border-secondary">
                    <Link to={"/gamelist/" + this.state.game.id}><img src={this.state.game.ImageUrl} className="card-img-top img-fluid" style={{height:"10rem", objectFit:"cover"}}alt="..." /></Link>
                    <div className="card-body p-0 d-flex">
                        <h5 className="card-title m-3 flex-grow-1 align-self-center text-truncate">{this.state.game.Name}</h5>
                        {this.props.isAuthenticated ? this.props.isLibrary ? this.planButton: null : null}
                        {
                            this.props.isAuthenticated ? this.state.game.IsOwned ? this.removeButton : this.addButton:null
                        }
                    </div>
                </div>
            </div>
        );
    }
}
