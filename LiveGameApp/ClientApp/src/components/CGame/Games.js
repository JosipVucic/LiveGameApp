import React, { Component } from 'react';
import DataProvider from '../admin/DataProvider'
import authService from '../api-authorization/AuthorizeService';
import { FilterPill } from '../CUtil/FilterPill';
import { GameAlbum } from './GameAlbum'

export class Games extends Component {
    static displayName = Games.name;

    constructor(props) {
        super(props);
        this.state = {
            games: [],
            total: 0,
            pagination: {
                page: 1,
                perPage: 12,
                hasPrev: false,
                hasNext: false
            },
            filter:
            {
                q: undefined,
                GenreIds: []
            },
            genres: [],
            selectedGenres: [],
            filterMode: false,
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
            pagination: this.state.pagination,
            sort: {
                field: "id",
                order: "ASC"
            },
            filter: this.state.filter
        };
        let pagination = this.state.pagination;
        let hasPrev = pagination.page > 1;
        let startIndex = (pagination.page) * pagination.perPage;
        DataProvider.getList("CGames", params)
            .then(object => this.setState(
                {
                    ...this.state,
                    pagination:
                    {
                        ...this.state.pagination,
                        hasPrev: hasPrev,
                        hasNext: startIndex < object.total
                    },
                    games: object.data,
                    total: object.total
                }));

        const genreParams = {
            ...params,
            filter: undefined,
            pagination: {
                ...this.state.pagination,
                perPage: 100,
            }
        }

        DataProvider.getList("GameGenres", genreParams)
            .then(object => {
                this.setState(
                    {
                        ...this.state,
                        genres: object.data
                    });
            });
    }

    handlePageChange = (event) => {
        this.setState(
            {
                ...this.state,
                pagination:
                {
                    ...this.state.pagination,
                    page: event.target.value
                }
            }, () => this.fetchItems());
    }

    handlePageInc = (event) => {
        event.preventDefault();
        let page = this.state.pagination.page + 1;
        this.setState(
            {
                ...this.state,
                pagination:
                {
                    ...this.state.pagination,
                    page: page
                }
            }, () => this.fetchItems());
    }

    handlePageDec = (event) => {
        event.preventDefault();
        let page = Math.max(this.state.pagination.page - 1, 1);
        this.setState(
            {
                ...this.state,
                pagination:
                {
                    ...this.state.pagination,
                    page: page
                }
            }, () => this.fetchItems());
    }

    handleGenreAdd = (event) => {
        event.preventDefault();

        let filter = this.state.filter;
        let genre = JSON.parse(event.target.value);
        let GenreIds = filter.GenreIds;
        let selectedGenres = this.state.selectedGenres;
        if (!GenreIds.includes(genre.id)) {
            GenreIds.push(genre.id);
            selectedGenres.push(genre);
        }

        this.setState(
            {
                ...this.state,
                filter:
                {
                    ...filter,
                    GenreIds: GenreIds
                },
                selectedGenres: selectedGenres
            }, () => this.fetchItems());
    }

    handleFilterClose = (event) => {
        let filter = this.state.filter;

        let GenreIds = filter.GenreIds;
        let selectedGenres = this.state.selectedGenres;

        GenreIds = GenreIds.filter((value) => value != event.currentTarget.value);
        selectedGenres = selectedGenres.filter((value) => value.id != event.currentTarget.value);

        this.setState(
            {
                ...this.state,
                filter:
                {
                    ...filter,
                    GenreIds: GenreIds
                },
                selectedGenres: selectedGenres
            }, () => this.fetchItems());
    }

    componentDidMount() {
        this._subscription = authService.subscribe(() => this.populateState());
        this.populateState();
    }

    componentWillUnmount() {
        authService.unsubscribe(this._subscription);
    }

    filterMenu = null;

    handleFilterMenu = (event) => {
        event.preventDefault();

        const filterMode = !this.state.filterMode;
        this.setState(
            {
                ...this.state,
                filterMode: filterMode
            });
    }

    handleReset = (event) => {
        event.preventDefault();

        this.setState({
            ...this.state,
            filter:
            {
                q: "",
                GenreIds: []
            },
            selectedGenres: []
        }, () => this.fetchItems())

    }

    handleChange = (event) => {
        const target = event.target;
        let value = target.type === 'checkbox' ? target.checked : target.value;
        const name = target.name;

        if (value == "") value = undefined;

        const filter = { ...this.state.filter, [name]: value };

        this.setState({
            ...this.state,
            filter: filter,
            pagination: {
                page: 1,
                perPage: 12,
                hasPrev: false,
                hasNext: false,
            }
        }, () => this.fetchItems());
    }

    render() {

        this.filterMenu =
            <div className="d-flex m-3 align-items-center justify-content-center">
                <div className="card bg-dark col-sm-12 col-md-6 col-lg-5">
                    <div className="card-body px-3 pt-3 pb-0">
                        <form>
                            <div className="form-group">
                                <label for="GameName">Name</label>
                                <input type="text" name="q" id="GameName" value={this.state.filter.q} onChange={this.handleChange} className="form-control w-75" />
                            </div>
                            <div className="form-group">
                                <label for="GameGenres">Add Genre</label>
                                <select name="Type" onChange={this.handleGenreAdd} className="form-control w-50" id="GameGenres">
                                    <option selected value="" disabled >Undefined</option>
                                    {this.state.genres.map((value, index) => {
                                        return <option value={JSON.stringify(value)}>{value.Name}</option>
                                    })}
                                </select>
                                {this.state.selectedGenres.map((value, index) => {
                                    return <FilterPill Name={value.Name} Value={value.id} close={this.handleFilterClose} />
                                })}
                            </div>
                            <div className="form-group d-flex">
                                <button className="form-control btn mx-1 btn-secondary" onClick={this.handleReset}>Reset</button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>;

        return (

            <div className="container-fluid">
                <div className="text-center m-5">
                    <h1 className="display-4">Games</h1>
                </div>

                <nav aria-label="Page navigation">
                    <form>

                        <ul className="pagination justify-content-center">
                            <li className="page-item">
                                <button onClick={this.handleFilterMenu} className="page-link bg-dark text-light">Filters</button>
                            </li>
                            <li className="page-item">
                                <button className="page-link" disabled={!this.state.pagination.hasPrev} onClick={this.handlePageDec}>Previous</button>
                            </li>
                            <li className="page-item">
                                <input type="number" className="page-link text-center" value={this.state.pagination.page} onChange={this.handlePageChange} />
                            </li>
                            <li className="page-item">
                                <button className="page-link" disabled={!this.state.pagination.hasNext} onClick={this.handlePageInc}>Next</button>
                            </li>

                        </ul>
                    </form>
                </nav>

                {this.state.filterMode ? this.filterMenu : null}

                <GameAlbum games={this.state.games} isAuthenticated={this.state.isAuthenticated} click={() => { }} AddBtnName="Add" RemoveBtnName="Remove" PlanBtnName="Plan" />

            </div>
        );
    }
}
