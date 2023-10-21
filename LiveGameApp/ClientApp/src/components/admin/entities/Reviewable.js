import * as React from "react";
import {
    List,
    Datagrid,
    TextField,
    Edit,
    Create,
    SimpleForm,
    TextInput,
    Filter
} from 'react-admin';

export const ReviewableList = props => (
    <List filters={<ReviewableFilter />} {...props}>
        <Datagrid rowClick="edit">
            <TextField source="id" />
            <TextField source="AverageRating" />
        </Datagrid>
    </List>
);

export const ReviewableEdit = props => (
    <Edit {...props}>
        <SimpleForm>
            <TextInput disabled source="id" />
            <TextInput source="AverageRating" />
        </SimpleForm>
    </Edit>
);

export const ReviewableCreate = props => (
    <Create {...props}>
        <SimpleForm>
            <TextInput source="AverageRating" />
        </SimpleForm>
    </Create>
);

export const ReviewableFilter = (props) => (
    <Filter {...props}>
        <TextInput label="Search" source="q" alwaysOn />
    </Filter>
);