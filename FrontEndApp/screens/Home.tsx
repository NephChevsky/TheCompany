import React from 'react';
import { Text, View, StyleSheet } from 'react-native';
import { useMyContext } from '../services/Context';

export default function HomeScreen() {
	const { currentUser } = useMyContext();
	return (
		<View>
			<Text>{JSON.stringify(currentUser)}</Text>
		</View>
	);
}