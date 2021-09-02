import React from 'react';
import { Text, View, StyleSheet } from 'react-native';
import { useMyContext } from '../services/Context';
import { NativeStackScreenProps } from "@react-navigation/native-stack";
import { RootStackParamList } from "../types/routes";

type Props = NativeStackScreenProps<RootStackParamList, 'Home'>;

export default function Home({ route, navigation }: Props) {
	const { currentUser } = useMyContext();
	return (
		<View>
			<Text>{JSON.stringify(currentUser)}</Text>
		</View>
	);
}