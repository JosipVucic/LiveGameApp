import React, { Component } from 'react';
import { GameCard } from './GameCard';

export class GameAlbum extends Component {
    static displayName = GameAlbum.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div className="container-fluid">
                <div className="d-flex flex-wrap align-items-stretch">
                    {this.props.games.map((value, index) => {
                        return <GameCard game={value} plan={this.props.plan} isAuthenticated={this.props.isAuthenticated} isLibrary={this.props.isLibrary} click={this.props.click} AddBtnName={this.props.AddBtnName} RemoveBtnName={this.props.RemoveBtnName} PlanBtnName={this.props.PlanBtnName} />
                    })}
                </div>
            </div>
        );
    }
}
