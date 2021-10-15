import React, { useState } from "react";
import { NavigationContainer, DefaultTheme } from '@react-navigation/native';
import { createDrawerNavigator } from '@react-navigation/drawer';
import {CompanyName} from "@env";
import Login from "./screens/Login";
import Home from "./screens/Home";
import { Context, useMyContext } from "./services/Context";
import AsyncStorage from "@react-native-async-storage/async-storage";
import Customers from "./screens/Customers";

const Drawer = createDrawerNavigator();

const myTheme = {
	...DefaultTheme,
	colors: {
		...DefaultTheme.colors,
		background: 'white'
	},
};

export default function App() {
	const currentContext = useMyContext();
	const [currentUser, setCurrentUser] = useState(async () => {
        const data = await AsyncStorage.getItem("currentUser");
        setCurrentUser(typeof data === "string" ? JSON.parse(data) : data);
      }
    );
	return (
		<Context.Provider value={{currentUser, setCurrentUser}}>
			<NavigationContainer theme={myTheme}>
				<Drawer.Navigator>
					{
						currentContext.currentUser ? null : 
						(
							<Drawer.Screen name="Login" component={Login} />
						)
					}
					{
						currentContext.currentUser ? 
						(
							<Drawer.Screen name="Home" component={Home} />
						) : null
					}
					{
						currentContext.currentUser ?
						(
							<Drawer.Screen name="Customers" component={Customers} />
						) : null
					}
					{
						currentContext.currentUser ?
						(
							<Drawer.Screen name="Logout" component={Login} />
						) : null
					}
				</Drawer.Navigator>
			</NavigationContainer>
		</Context.Provider>
	);
}
