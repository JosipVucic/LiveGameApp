import * as React from "react";
import { DateTimeInput as DTInput, DateInput as DInput, DateField as DField } from 'react-admin';

export const DateTimeInput = props => (
    <DTInput locales='hr-HR' {...props} />
);
export const DateInput = props => (
    <DInput locales='hr-HR' {...props}/>
);
export const DateField = props => (
    <DField locales='hr-HR' {...props}/>
);
