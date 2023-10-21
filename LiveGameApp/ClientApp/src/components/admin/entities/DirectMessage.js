import * as React from "react";
import {
    List,
    Datagrid,
    TextField,
    ReferenceField,
    EditButton,
    Edit,
    Create,
    SimpleForm,
    ReferenceInput,
    SelectInput,
    TextInput,
    Filter
} from 'react-admin';
import { DateField, DateTimeInput } from '../custom/CustomDates';

export const DirectmessageList = props => (
    <List filters={<DirectmessageFilter />} {...props}>
        <Datagrid rowClick="edit">
            <TextField source="id" />
            <ReferenceField source="SenderId" reference="AppUsers"><TextField source="Username" /></ReferenceField>
            <ReferenceField source="RecipientId" reference="AppUsers"><TextField source="Username" /></ReferenceField>
            <DateField source="Datetime" showTime />
            <TextField source="Content" />
            <EditButton/>
        </Datagrid>
    </List>
);

export const DirectmessageEdit = props => (
    <Edit {...props}>
        <SimpleForm>
            <TextInput disabled source="id" />
            <ReferenceInput source="SenderId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="RecipientId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <DateTimeInput source="Datetime" />
            <TextInput source="Content" />
        </SimpleForm>
    </Edit>
);

export const DirectmessageCreate = props => (
    <Create {...props}>
        <SimpleForm>
            <ReferenceInput source="SenderId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="RecipientId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <DateTimeInput source="Datetime" />
            <TextInput source="Content" />
        </SimpleForm>
    </Create>
);

export const DirectmessageFilter = (props) => (
    <Filter {...props}>
        <TextInput label="Search" source="q" alwaysOn />
    </Filter>
);