import React from "react";
import { NavigationContainer, DefaultTheme } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import {CompanyName} from "@env";
import LoginScreen from "./screens/Login";
import HomeScreen from "./screens/Home";

const Stack = createNativeStackNavigator();

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
			LoginScreen: "Login",
			HomeScreen: "Home"
		}
	  },
}

export default function App() {
	return (
		<NavigationContainer theme={myTheme} linking={linking}>
			<Stack.Navigator screenOptions={{headerShown: false}}>
			<Stack.Screen
				name="LoginScreen"
				component={LoginScreen}
				options={{ title: CompanyName + ' - Login'}}
			/>
			<Stack.Screen
				name="HomeScreen"
				component={HomeScreen}
				options={{ title: CompanyName + ' - Home'}}
				/>
			</Stack.Navigator>
		</NavigationContainer>
	);
}
