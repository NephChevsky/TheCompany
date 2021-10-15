import React from 'react';
import { SafeAreaView } from 'react-native';
import { NativeStackScreenProps } from "@react-navigation/native-stack";
import { RootStackParamList } from "../types/routes";
import { useMyContext } from '../services/Context';

type Props = NativeStackScreenProps<RootStackParamList, 'Home'>;

export default function Home({ route, navigation }: Props) {
	const myContext = useMyContext();
	return (
		<SafeAreaView>
			
		</SafeAreaView>
	);
}