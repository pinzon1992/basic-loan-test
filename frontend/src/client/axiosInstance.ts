import axios, { type AxiosRequestConfig } from 'axios';
import { environment } from '../environments/environment';

const instance = axios.create({
  baseURL: environment.apiBaseUrl || '', 
  withCredentials: true, 
});

export async function axiosInstance<T = any>(config: AxiosRequestConfig): Promise<T> {
  const resp = await instance.request<T>(config);
  return resp.data as T;
}

export default instance;