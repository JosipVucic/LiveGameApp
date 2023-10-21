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
    Filter,
    NumberField,
    NumberInput,
    EditButton
} from 'react-admin';

export const ReviewList = props => (
    <List filters={<ReviewFilter/> } {...props}>
        <Datagrid rowClick="edit">
            <TextField source="id" />
            <ReferenceField source="UserId" reference="AppUsers"><TextField source="Username" /></ReferenceField>
            <ReferenceField source="ReviewableId" reference="Reviewables"><TextField source="id" /></ReferenceField>
            <NumberField source="Rating" />
            <TextField source="Content" />
            <EditButton />
        </Datagrid>
    </List>
);

export const ReviewEdit = props => (
    <Edit {...props}>
        <SimpleForm>
            <TextInput disabled source="id" />
            <ReferenceInput disabled source="UserId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput disabled source="ReviewableId" reference="Reviewables"><SelectInput optionText="id" /></ReferenceInput>
            <NumberInput source="Rating" />
            <TextInput source="Content" />
        </SimpleForm>
    </Edit>
);

export const ReviewCreate = props => (
    <Create {...props}>
        <SimpleForm>
            <ReferenceInput source="UserId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="ReviewableId" reference="Reviewables"><SelectInput optionText="id" /></ReferenceInput>
            <NumberInput source="Rating" />
            <TextInput source="Content" />
        </SimpleForm>
    </Create>
);

export const ReviewFilter = (props) => (
    <Filter {...props}>
        <TextInput label="Search" source="q" alwaysOn />
    </Filter>
);