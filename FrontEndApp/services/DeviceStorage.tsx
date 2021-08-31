import AsyncStorage from '@react-native-async-storage/async-storage';

const DeviceStorage = {
	async saveItem(key: string, value: any) {
		try {
		await AsyncStorage.setItem(key, value);
		} catch (error) {
		console.log("[AsyncStorage] Couldn't save key '" + key + "': " + error.message);
		}
	}

};

export default DeviceStorage;