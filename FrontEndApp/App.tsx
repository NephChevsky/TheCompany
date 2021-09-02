import React, { useState } from "react";
import { NavigationContainer, DefaultTheme } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import {CompanyName} from "@env";
import Login from "./screens/Login";
import Home from "./screens/Home";
import { Context } from "./services/Context";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { RootStackParamList } from "./types/routes";

const Stack = createNativeStackNavigator<RootStackParamList>();

const myTheme = {
	...DefaultTheme,
	colors: {
	  ...DefaultTheme.colors,
	  background: 'white'
	},
  };

const linking = {
	prefixes: [],
	  config: {
		screens: {
			Login: "Login",
			Home: "Home"
		}
	  },
}

export default function App() {
	const [currentUser, setCurrentUser] = useState(async () => {
        const data = await AsyncStorage.getItem("currentUser");
        setCurrentUser(typeof data === "string" ? JSON.parse(data) : data);
      }
    );
	return (
		<Context.Provider value={{currentUser, setCurrentUser}}>
			<NavigationContainer theme={myTheme} linking={linking}>
				<Stack.Navigator screenOptions={{headerShown: false}}>
				<Stack.Screen
					name="Login"
					component={Login}
					options={{ title: CompanyName + ' - Login'}}
				/>
				<Stack.Screen
					name="Home"
					component={Home}
					options={{ title: CompanyName + ' - Home'}}
					/>
				</Stack.Navigator>
			</NavigationContainer>
		</Context.Provider>
	);
}
