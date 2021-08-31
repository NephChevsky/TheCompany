import { StatusBar } from "expo-status-bar";
import React, { useState } from "react";
import { NavigationContainer, DefaultTheme } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import {CompanyName} from "@env";
import LoginScreen from "./screens/Login";

const Stack = createNativeStackNavigator();

const MyTheme = {
	...DefaultTheme,
	colors: {
	  ...DefaultTheme.colors,
	  background: 'white'
	},
  };

export default function App() {
	return (
		<NavigationContainer theme={MyTheme}>
			<Stack.Navigator screenOptions={{headerShown: false}}>
			<Stack.Screen
				name="LoginScreen"
				component={LoginScreen}
				options={{ title: CompanyName + ' - Login' }}
			/>
			</Stack.Navigator>
		</NavigationContainer>
	);
}
