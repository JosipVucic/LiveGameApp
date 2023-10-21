import * as React from "react";
import {
    List,
    Datagrid,
    TextField,
    ReferenceField,
    Edit,
    Create,
    SimpleForm,
    ReferenceInput,
    SelectInput,
    TextInput,
    Filter
} from 'react-admin';

export const IsGenreList = props => (
    <List filters={<IsGenreFilter/> } {...props}>
        <Datagrid rowClick="edit">
            <TextField source="id" />
            <ReferenceField source="GameId" reference="Games"><TextField source="Name" /></ReferenceField>
            <ReferenceField source="GenreId" reference="GameGenres"><TextField source="Name" /></ReferenceField>
        </Datagrid>
    </List>
);

export const IsGenreEdit = props => (
    <Edit {...props}>
        <SimpleForm>
            <TextInput disabled source="id" />
            <ReferenceInput source="GameId" reference="Games"><SelectInput optionText="Name" /></ReferenceInput>
            <ReferenceInput source="GenreId" reference="GameGenres"><SelectInput optionText="Name" /></ReferenceInput>
        </SimpleForm>
    </Edit>
);

export const IsGenreCreate = props => (
    <Create {...props}>
        <SimpleForm>
            <ReferenceInput source="GameId" reference="Games"><SelectInput optionText="Name" /></ReferenceInput>
            <ReferenceInput source="GenreId" reference="GameGenres"><SelectInput optionText="Name" /></ReferenceInput>
        </SimpleForm>
    </Create>
);

export const IsGenreFilter = (props) => (
    <Filter {...props}>
        <TextInput label="Search" source="q" alwaysOn />
    </Filter>
);