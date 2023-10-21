import React, { Component } from 'react';
import { Link } from 'react-router-dom';

export class Home extends Component {
    static displayName = Home.name;

    render() {
        return (
            <div className="container-fluid">
                <div className="text-center m-5">
                    <h1 className="display-4">LiveGameApp</h1>
                </div>

                <div className="container-fluid">
                    <div className="row">
                        <div className="col-md-4">
                            <div className="card bg-dark mx-3 border-secondary">
                                <img src="https://cdn.pixabay.com/photo/2017/11/11/03/09/chess-2938267_960_720.png" className="card-img-top" style={{ height: "22rem", objectFit: "cover" }} alt="..." />
                                <div className="card-body">
                                    <h5 className="card-title">Choose a game</h5>
                                    <p className="card-text">Add the games you own to your library. You can rate them and leave a review.</p>
                                    <Link to="/gamelist"><button className="btn btn-primary">All games</button></Link>
                                </div>
                            </div>
                        </div>
                        <div className="col-md-4">
                            <div className="card bg-dark mx-3 border-secondary">
                                <img src="https://cdn.pixabay.com/photo/2016/10/23/17/06/calendar-1763587_960_720.png" className="card-img-top" style={{ height: "22rem", objectFit: "cover" }} alt="..." />
                                <div className="card-body">
                                    <h5 className="card-title">Schedule your plans</h5>
                                    <p className="card-text">Choose between making a game plan or a tournament. Invite your friends or join a public plan.</p>
                                    <Link to="/library"><button className="btn btn-primary">Create a play plan</button></Link>
                                </div>
                            </div>
                        </div>
                        <div className="col-md-4">
                            <div className="card bg-dark mx-3 border-secondary">
                                <img src="https://cdn.pixabay.com/photo/2017/11/10/04/47/write-2935375_960_720.png" className="card-img-top" style={{ height: "22rem", objectFit: "cover" }} alt="..." />
                                <div className="card-body">
                                    <h5 className="card-title">Become a creator</h5>
                                    <p className="card-text">Make your own custom game and add it to the list. Your game your rules</p>
                                    <Link to="/gamecreate"><button className="btn btn-primary">Create a game</button></Link>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        );
    }
}
