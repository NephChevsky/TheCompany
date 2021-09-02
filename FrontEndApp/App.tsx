import React, { useState } from "react";
import { NavigationContainer, DefaultTheme } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import DeviceStorage from "./services/DeviceStorage";
import {CompanyName} from "@env";
import LoginScreen from "./screens/Login";
import HomeScreen from "./screens/Home";
import { Context } from "./services/Context";
import AsyncStorage from "@react-native-async-storage/async-storage";

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
		</Context.Provider>
	);
}
