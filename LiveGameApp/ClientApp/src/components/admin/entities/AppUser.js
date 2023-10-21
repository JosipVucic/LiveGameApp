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
import { DateField, DateInput} from '../custom/CustomDates';

export const AppuserList = props => (
    <List filters={<AppuserFilter />} {...props}>
        <Datagrid rowClick="edit">
            <TextField source="id" />
            <TextField source="Username" />
            <TextField source="Email" />
            <TextField source="Password" />
            <DateField source="DateOfBirth" />
            <EditButton />
        </Datagrid>
    </List>
);

export const AppuserEdit = props => (
    <Edit {...props}>
        <SimpleForm>
            <TextInput disabled source="id" />
            <TextInput source="Username" />
            <TextInput source="Email" />
            <TextInput source="Password" />
            <DateInput source="DateOfBirth" />
        </SimpleForm>
    </Edit>
);

export const AppuserCreate = props => (
    <Create {...props}>
        <SimpleForm>
            <TextInput source="Username" />
            <TextInput source="Email" />
            <TextInput source="Password" />
            <DateInput source="DateOfBirth" />
        </SimpleForm>
    </Create>
);

export const AppuserFilter = (props) => (
    <Filter {...props}>
        <TextInput label="Search" source="q" alwaysOn />
    </Filter>
);