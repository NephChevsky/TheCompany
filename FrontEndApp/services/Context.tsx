import { createContext, useContext, useState } from 'react';
import { User } from '../models/User';

export type ContextType = {
    currentUser: {};
    setCurrentUser: (user: User) => void;
}

export const Context = createContext<ContextType>({ currentUser: {}, setCurrentUser: user => {}});
export const useMyContext = () => useContext(Context);