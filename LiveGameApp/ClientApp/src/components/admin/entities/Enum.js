import * as React from "react";
import {
    List,
    Datagrid,
    TextField,
    EditButton,
    Edit,
    Create,
    SimpleForm,
    TextInput,
    Filter
} from 'react-admin';

export const EnumList = props => (
    <List filters={<EnumFilter/>} {...props}>
        <Datagrid rowClick="edit">
            <TextField source="id" />
            <TextField source="Name" />
            <EditButton />
        </Datagrid>
    </List>
);

export const EnumEdit = props => (
    <Edit {...props}>
        <SimpleForm>
            <TextInput disabled source="id" />
            <TextInput source="Name" />
        </SimpleForm>
    </Edit>
);

export const EnumCreate = props => (
    <Create {...props}>
        <SimpleForm>
            <TextInput source="Name" />
        </SimpleForm>
    </Create>
);

export const EnumFilter = (props) => (
    <Filter {...props}>
        <TextInput label="Search" source="q" alwaysOn />
    </Filter>
);