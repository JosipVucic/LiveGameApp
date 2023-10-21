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

export const FriendrequestList = props => (
    <List filters={<FriendrequestFilter/>} {...props}>
        <Datagrid rowClick="edit">
            <TextField source="id" />
            <ReferenceField source="SenderId" reference="AppUsers"><TextField source="Username" /></ReferenceField>
            <ReferenceField source="RecipientId" reference="AppUsers"><TextField source="Username" /></ReferenceField>
            <TextField source="Message" />
            <EditButton />
        </Datagrid>
    </List>
);

export const FriendrequestEdit = props => (
    <Edit {...props}>
        <SimpleForm>
            <TextInput disabled source="id" />
            <ReferenceInput source="SenderId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="RecipientId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <TextInput source="Message" />
        </SimpleForm>
    </Edit>
);

export const FriendrequestCreate = props => (
    <Create {...props}>
        <SimpleForm>
            <ReferenceInput source="SenderId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="RecipientId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <TextInput source="Message" />
        </SimpleForm>
    </Create>
);

export const FriendrequestFilter = (props) => (
    <Filter {...props}>
        <TextInput label="Search" source="q" alwaysOn />
    </Filter>
);