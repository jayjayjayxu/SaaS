# --- 阶段1: 构建阶段 ---
# 使用一个包含Node.js环境的轻量级镜像来构建我们的项目
FROM node:22-alpine AS build

# 设置工作目录
WORKDIR /app

# 复制package.json和package-lock.json，并安装依赖
# 这样可以利用Docker的缓存，如果依赖没变，就不用每次都重新安装
COPY package*.json ./
RUN npm install

# 复制所有剩余的前端源代码
COPY . .

# 运行构建命令，这会生成最终的静态文件到/app/dist目录
RUN npm run build

# --- 阶段2: 生产阶段 ---
# 使用一个超轻量级的Nginx服务器来托管我们构建好的静态文件
FROM nginx:stable-alpine

# 将构建阶段生成的/app/dist目录下的所有文件，复制到Nginx的默认网站根目录
COPY --from=build /app/dist /usr/share/nginx/html

# 复制我们自定义的Nginx配置文件（下一步会创建它）
COPY nginx.conf /etc/nginx/conf.d/default.conf

# 暴露Nginx的默认端口80
EXPOSE 80

# 容器启动时运行的命令：启动Nginx服务器
CMD ["nginx", "-g", "daemon off;"]