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
    Filter,
    NumberField,
    NumberInput,
    useRecordContext
} from 'react-admin';

const CustomTextField = ({ source }) => {
    const record = useRecordContext();
    let str = record && record[source];
    return record ? (<span> { str.length > 20 ? str.slice(0, 20) + "..." : str }</span>) : null;
};

export const GameList = props => (
    <List filters={<GameFilter />} {...props}>
        <Datagrid rowClick="edit">
            <TextField source="id" />
            <TextField source="Name" />
            <CustomTextField noWrap source="Description" />
            <CustomTextField noWrap source="Rules" />
            <NumberField source="MinPlayers" className="d-flex justify-content-center" />
            <NumberField source="MaxPlayers" className="d-flex justify-content-center" />
            <CustomTextField source="ImageUrl" />
            <EditButton />
        </Datagrid>
    </List>
);

export const GameEdit = props => (
    <Edit {...props}>
        <SimpleForm>
            <TextInput disabled source="id" />
            <TextInput source="Name" />
            <TextInput multiline source="Description" />
            <TextInput multiline source="Rules" />
            <NumberInput source="MinPlayers" />
            <NumberInput source="MaxPlayers" />
            <TextInput source="ImageUrl" />
        </SimpleForm>
    </Edit>
);

export const GameCreate = props => (
    <Create {...props}>
        <SimpleForm>
            <TextInput source="Name" />
            <TextInput multiline source="Description" />
            <TextInput multiline source="Rules" />
            <NumberInput source="MinPlayers" />
            <NumberInput source="MaxPlayers" />
            <TextInput source="ImageUrl" />
        </SimpleForm>
    </Create>
);

export const GameFilter = (props) => (
    <Filter {...props}>
        <TextInput label="Search" source="q" alwaysOn />
    </Filter>
);