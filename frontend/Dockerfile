# 阶段1: 直接使用Nginx服务器
FROM nginx:stable-alpine

# 将我们之前上传的dist文件夹里的所有内容，复制到Nginx的网站根目录
COPY ./dist /usr/share/nginx/html

# 复制Nginx配置文件以支持单页应用路由
COPY nginx.conf /etc/nginx/conf.d/default.conf

# 暴露80端口
EXPOSE 80

# 启动Nginx
CMD ["nginx", "-g", "daemon off;"]
