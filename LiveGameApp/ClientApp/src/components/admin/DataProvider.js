import { fetchUtils } from 'react-admin';
import { stringify } from 'query-string';
import authService from '../api-authorization/AuthorizeService';

const apiUrl = 'https://localhost:5001';
const httpClient = fetchUtils.fetchJson;

export default {
    getList: (resource, params) => {
        const { page, perPage } = params.pagination;
        const { field, order } = params.sort;
        const query = {
            field: field,
            order: order,
            page: page,
            perPage: perPage,
            filter: JSON.stringify(params.filter),
        };
        const url = `${apiUrl}/${resource}?${stringify(query)}`;

        return authService.getAccessToken().then(token => {
            return {
                headers: new Headers({
                    Accept: 'application/json',
                    Authorization: `Bearer ${token}`,
                }),
            }
        }).then(options => httpClient(url, options)).then(({ headers, json }) => ({
            data: json,
            total: parseInt(headers.get('content-range').split('/').pop(), 10),
        }));

    },

    getOne: (resource, params) =>

        authService.getAccessToken().then(token => {
            return {
                headers: new Headers({
                    Accept: 'application/json',
                    Authorization: `Bearer ${token}`,
                }),
            }
        }).then(options => httpClient(`${apiUrl}/${resource}/${params.id}`, options)).then(({ json }) => ({
            data: json,
        })),

    getMany: (resource, params) => {
        const query = {
            ids: JSON.stringify(params.ids),
        };
        const url = `${apiUrl}/${resource}?${stringify(query)}`;
        return authService.getAccessToken().then(token => {
            return {
                headers: new Headers({
                    Accept: 'application/json',
                    Authorization: `Bearer ${token}`,
                }),
            }
        }).then(options => httpClient(url, options)).then(({ json }) => ({
            data: json,
        }));
    },

    getManyReference: (resource, params) => {
        const { page, perPage } = params.pagination;
        const { field, order } = params.sort;
        const query = {
            field: field,
            order: order,
            page: page,
            perPage: perPage,
            filter: JSON.stringify(params.filter),
            target: params.target,
            tid: params.id
        };
        const url = `${apiUrl}/${resource}?${stringify(query)}`;

        return authService.getAccessToken().then(token => {
            return {
                headers: new Headers({
                    Accept: 'application/json',
                    Authorization: `Bearer ${token}`,
                }),
            }
        }).then(options => httpClient(url, options)).then(({ headers, json }) => ({
            data: json,
            total: parseInt(headers.get('content-range').split('/').pop(), 10),
        }));
    },

    update: (resource, params) =>
        authService.getAccessToken().then(token => {
            return {
                headers: new Headers({
                    Accept: 'application/json',
                    Authorization: `Bearer ${token}`,
                }),
                method: 'PUT',
                body: JSON.stringify(params.data),
            }
        }).then(options => httpClient(`${apiUrl}/${resource}/${params.id}`, options)).then(({ json }) => ({
            data: json.Value,
        })),

    updateMany: (resource, params) => {
        const query = {
            ids: JSON.stringify(params.ids),
        };
        return authService.getAccessToken().then(token => {
            return {
                headers: new Headers({
                    Accept: 'application/json',
                    Authorization: `Bearer ${token}`,
                }),
                method: 'PUT',
                body: JSON.stringify(params.data),
            }
        }).then(options => httpClient(`${apiUrl}/${resource}?${stringify(query)}`, options)).then(({ json }) => ({
            data: json,
        }));
    },

    create: (resource, params) =>
        authService.getAccessToken().then(token => {
            return {
                headers: new Headers({
                    Accept: 'application/json',
                    Authorization: `Bearer ${token}`,
                }),
                method: 'POST',
                body: JSON.stringify(params.data),
            }
        }).then(options => httpClient(`${apiUrl}/${resource}`, options)).then(({ json }) => ({
            data: {...params.data, id: json.id },
        })),

    delete: (resource, params) =>
        authService.getAccessToken().then(token => {
            return {
                headers: new Headers({
                    Accept: 'application/json',
                    Authorization: `Bearer ${token}`,
                }),
                method: 'DELETE',
            }
        }).then(options => httpClient(`${apiUrl}/${resource}/${params.id}`, options)).then(({ json }) => ({
            data: json,
        })),

    deleteMany: (resource, params) => {
        const query = {
            ids: JSON.stringify(params.ids),
        };
        return authService.getAccessToken().then(token => {
            return {
                headers: new Headers({
                    Accept: 'application/json',
                    Authorization: `Bearer ${token}`,
                }),
                method: 'DELETE',
            }
        }).then(options => httpClient(`${apiUrl}/${resource}?${stringify(query)}`, options)).then(({ json }) => ({
            data: json,
        }));
    }
};