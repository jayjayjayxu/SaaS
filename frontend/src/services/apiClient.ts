import axios from 'axios';

// 创建一个axios实例，并配置基础URL为我们的API网关地址
const apiClient = axios.create({
  // 使用你服务器的公网IP和网关端口
  baseURL: 'http://115.190.155.213:7077', 
});

// ▼▼▼ 添加一个请求拦截器（Request Interceptor） ▼▼▼
apiClient.interceptors.request.use(
  config => {
    // 在每个请求发送前，都来这里检查一下
    const token = localStorage.getItem('jwt_token');
    if (token) {
      // 如果本地存储中有Token，就把它加到请求的Authorization头里
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => {
    // 处理请求错误
    return Promise.reject(error);
  }
);
apiClient.interceptors.response.use(
  response => response,
  error => {
    if (error.response && error.response.status === 401) {
      // 检查这个401错误是不是来自登录API本身
      // 如果是，我们就不在这里处理，直接把错误抛出去给登录页面自己处理
      if (error.config.url.endsWith('/identity/api/auth/login')) {
        return Promise.reject(error);
      }
      // 如果是来自其他任何API的401错误，我们才认为是Token过期
      localStorage.removeItem('jwt_token');
      alert('您的登录已过期，请重新登录。');
      window.location.href = '/login'; 
    }
    return Promise.reject(error);
  }
);

export default apiClient;