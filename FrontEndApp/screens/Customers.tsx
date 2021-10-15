import React from 'react';
import { SafeAreaView } from 'react-native';
import { NativeStackScreenProps } from "@react-navigation/native-stack";
import { RootStackParamList } from "../types/routes";

type Props = NativeStackScreenProps<RootStackParamList, 'Customers'>;

export default function Customers({ route, navigation }: Props) {
	return (
		<SafeAreaView>
			
		</SafeAreaView>
	);
}