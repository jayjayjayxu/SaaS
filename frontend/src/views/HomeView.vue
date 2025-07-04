<template>
  <div class="home-container">
    <header class="home-header">
      <h1>我的项目</h1>
       <button @click="openCreateProjectModal" class="create-project-btn">创建新项目</button>
      <button @click="handleLogout" class="logout-button">登出</button>
    </header>

      <main class="projects-grid">
      <div v-if="loading" class="loading-indicator">
        正在加载项目...
      </div>

      <div v-else-if="projects.length > 0" class="project-list">
        <div v-for="project in projects" :key="project.id" class="project-card">
          
          <div class="project-card-header">
            <h2>{{ project.name }}</h2>
            <div class="project-actions">
              <button @click="openEditProjectModal(project)" class="action-btn">编辑</button>
              <button @click="deleteProject(project.id)" class="action-btn delete">删除</button>
            </div>
          </div>

          <p>{{ project.description || '暂无描述' }}</p>

          <div class="tasks-section">
            <h4>任务列表:</h4>
            <ul v-if="project.tasks.length > 0">
              <li v-for="task in project.tasks" :key="task.id" :class="{ 'task-completed': task.isCompleted }">
                <input 
                  type="checkbox" 
                  :checked="task.isCompleted" 
                  @change="toggleTask(project.id, task)"
                  class="task-checkbox"
                  :title="task.isCompleted ? '标记为未完成' : '完成此任务'"
                />
                <router-link :to="{ name: 'task-detail', params: { projectId: project.id, taskId: task.id } }">
                  {{ task.title }}
                </router-link>
                <button 
                  @click.stop="deleteTask(project, task.id)" 
                  class="delete-task-btn" 
                  title="删除此任务"
                >
                  &times;
                </button>
              </li>
            </ul>
            <p v-else class="no-tasks-message">这个项目下暂无任务</p>
          </div>
          <div class="add-task-button-container">
            <button @click="openAddTaskModal(project.id)" class="add-task-button">添加新任务</button>
          </div>
        </div>
      </div>
      
      <div v-else class="no-projects">
        你还没有创建任何项目。
      </div>
    </main>
  </div>
    <Modal :show="isModalOpen" @close="closeModal">
    <template #header>
      <h3>创建新任务</h3>
    </template>
    <template #body>
      <form @submit.prevent="handleAddTaskSubmit">
        <div class="form-group">
          <label for="task-title">标题</label>
          <input type="text" id="task-title" v-model="newTask.title" required>
        </div>
        <div class="form-group">
          <label for="task-desc">描述</label>
          <textarea id="task-desc" v-model="newTask.description" rows="4"></textarea>
        </div>
        <button type="submit" class="submit-task-button">确认添加</button>
      </form>
    </template>
  </Modal>
  <Modal :show="isProjectModalOpen" @close="closeProjectModal">
    <template #header>
      <h3>{{ isEditingProject ? '编辑项目' : '创建新项目' }}</h3>
    </template>
    <template #body>
      <form @submit.prevent="handleProjectSubmit">
        <div class="form-group">
          <label for="project-name">项目名称</label>
          <input type="text" id="project-name" v-model="currentProject.name" required>
        </div>
        <div class="form-group">
          <label for="project-desc">项目描述</label>
          <textarea id="project-desc" v-model="currentProject.description" rows="4"></textarea>
        </div>
        <button type="submit" class="submit-task-button">{{ isEditingProject ? '保存更改' : '确认创建' }}</button>
      </form>
    </template>
  </Modal>
</template>
<script setup lang="ts">
import { ref, onMounted,reactive } from 'vue';
import { useRouter } from 'vue-router';
import apiClient from '@/services/apiClient';
import Modal from '@/components/Modal.vue';
import { useAuthStore } from '@/stores/authStore'; 

const projects = ref<any[]>([]); 
const loading = ref(true);
const router = useRouter();

// --- Modal相关的状态 ---
const isModalOpen = ref(false);
const selectedProjectId = ref<string | null>(null);
const newTask = reactive({ title: '', description: '' });
// --- ▼▼▼ 为“项目”的CRUD新增的状态 ▼▼▼ ---
const isProjectModalOpen = ref(false);
const isEditingProject = ref(false);
// 使用reactive来处理表单数据
const currentProject = reactive({
  id: null,
  name: '',
  description: ''
});
const openAddTaskModal = (projectId: string) => {
  selectedProjectId.value = projectId;
  isModalOpen.value = true;
};

const closeModal = () => {
  isModalOpen.value = false;
  // 重置表单
  newTask.title = '';
  newTask.description = '';
  selectedProjectId.value = null;
};

const handleAddTaskSubmit = async () => {
  if (!newTask.title.trim()) {
    alert('任务标题不能为空！');
    return;
  }
  if (!selectedProjectId.value) return;

  try {
    const response = await apiClient.post(`/projects/api/projects/${selectedProjectId.value}/tasks`, {
      title: newTask.title,
      description: newTask.description
    });

    // 更新UI
    const project = projects.value.find(p => p.id === selectedProjectId.value);
    if (project) {
      project.tasks.push(response.data);
    }

    // 关闭并重置模态框
    closeModal();

  } catch (error) {
    console.error('添加任务失败:', error);
    alert('添加任务失败，请重试。');
  }
};

onMounted(async () => {
  try {
    const response = await apiClient.get('/projects/api/projects');
    // ▼▼▼ 修改这里：为每个项目添加一个用于v-model的空属性 ▼▼▼
    projects.value = response.data.map((p: any) => ({ ...p, newTaskTitle: '' }));
  } catch (error: any) {
    console.error('获取项目失败:', error);
    if (error.response && error.response.status === 401) {
      alert('您的会话已过期，请重新登录。');
      router.push({ name: 'login' });
    }
  } finally {
    loading.value = false;
  }
});

const handleAddTask = async (project: any) => {
  if (!project.newTaskTitle.trim()) {
    alert('任务标题不能为空！');
    return;
  }
  try {
    // 调用我们之前已经写好的创建任务API
    const response = await apiClient.post(`/projects/api/projects/${project.id}/tasks`, {
      title: project.newTaskTitle
    });

    // 成功后，直接将返回的新任务添加到前端的项目任务列表中，实现无刷新更新UI
    project.tasks.push(response.data);

    // 清空输入框
    project.newTaskTitle = '';

  } catch (error) {
    console.error('添加任务失败:', error);
    alert('添加任务失败，请重试。');
  }
};

const authStore = useAuthStore(); // 获取store实例

const handleLogout = () => {
  authStore.logout(); // 直接调用store的logout action
};
const toggleTask = async (projectId: string, task: any) => {
  // 这是一个很好的用户体验技巧，叫做“乐观更新”(Optimistic UI)。
  // 我们先立刻在界面上更新状态，让用户感觉操作立即生效。
  task.isCompleted = !task.isCompleted;

  try {
    // 然后在后台默默地调用API
    await apiClient.patch(`/projects/api/projects/${projectId}/tasks/${task.id}/toggle`);
  } catch (error) {
    // 如果API调用失败，我们就把界面的状态恢复回去，并提示用户
    console.error('更新任务状态失败:', error);
    alert('抱歉，更新任务失败，请重试。');
    task.isCompleted = !task.isCompleted; // 恢复UI状态
  }
};
const deleteTask = async (project: any, taskId: string) => {
  // 在执行危险操作前，给用户一个确认提示
  if (!confirm('你确定要永久删除这个任务吗？')) {
    return;
  }

  try {
    await apiClient.delete(`/projects/api/projects/${project.id}/tasks/${taskId}`);

    // API调用成功后，从前端的列表中移除这个任务，实现UI的即时更新
    const taskIndex = project.tasks.findIndex((t: any) => t.id === taskId);
    if (taskIndex > -1) {
      project.tasks.splice(taskIndex, 1);
    }

  } catch (error) {
    console.error('删除任务失败:', error);
    alert('删除任务失败，请重试。');
  }
};
// 打开“创建新项目”的模态框
const openCreateProjectModal = () => {
  isEditingProject.value = false;
  // 重置表单
  currentProject.id = null;
  currentProject.name = '';
  currentProject.description = '';
  isProjectModalOpen.value = true;
};

// 打开“编辑项目”的模态框
const openEditProjectModal = (project: any) => {
  isEditingProject.value = true;
  // 填充表单
  currentProject.id = project.id;
  currentProject.name = project.name;
  currentProject.description = project.description;
  isProjectModalOpen.value = true;
};

// 关闭模态框
const closeProjectModal = () => {
  isProjectModalOpen.value = false;
};

// 提交项目表单（处理创建和更新）
const handleProjectSubmit = async () => {
  if (!currentProject.name.trim()) {
    alert('项目名称不能为空！');
    return;
  }

  try {
    if (isEditingProject.value) {
      // --- 更新项目 ---
      await apiClient.put(`/projects/api/projects/${currentProject.id}`, {
        name: currentProject.name,
        description: currentProject.description
      });
      // 更新UI
      const projectInList = projects.value.find(p => p.id === currentProject.id);
      if (projectInList) {
        projectInList.name = currentProject.name;
        projectInList.description = currentProject.description;
      }
      alert('项目更新成功！');
    } else {
      // --- 创建新项目 ---
      const response = await apiClient.post('/projects/api/projects', {
        name: currentProject.name,
        description: currentProject.description
      });
      // 更新UI
      projects.value.push(response.data);
      alert('新项目创建成功！');
    }
    closeProjectModal();
  } catch (error) {
    console.error('项目操作失败:', error);
    alert('操作失败，请重试。');
  }
};

// 删除项目
const deleteProject = async (projectId: string) => {
  // 1. 根据ID找到我们要操作的那个项目对象
  const projectToDelete = projects.value.find(p => p.id === projectId);

  // 如果项目在前端列表中不存在，则不执行任何操作
  if (!projectToDelete) return; 

  // 2. 检查这个项目里是否存在 isCompleted 为 false 的任务
  // .some() 是一个高效的数组方法，只要找到一个满足条件的就会立刻返回true
  const hasUncompletedTasks = projectToDelete.tasks.some((task: { isCompleted: any; }) => !task.isCompleted);

  // 3. 如果存在未完成的任务，则弹窗提示并终止操作
  if (hasUncompletedTasks) {
    alert('无法删除！此项目下尚有未完成的任务。请先处理所有任务再删除项目。');
    return; // 关键：终止函数执行，不进行后续的删除操作
  }
  if (!confirm('你确定要永久删除这个项目吗？它将被彻底移除。')) {
    return;
  }
  try {
    await apiClient.delete(`/projects/api/projects/${projectId}`);
    projects.value = projects.value.filter(p => p.id !== projectId);
    alert('项目已删除。');
  } catch (error) {
    console.error('删除项目失败:', error);
    alert('删除项目失败，请重试。');
  }
};
</script>

<style scoped>
.home-container {
  width: 100%;
  max-width: 1280px;
  margin: 0 auto;
  padding: 2rem;
}
.home-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  border-bottom: 1px solid #eee;
  padding-bottom: 1rem;
}
h1 {
  font-size: 2rem;
  font-weight: 600;
}
.logout-button {
  padding: 8px 16px;
  font-size: 14px;
  color: white;
  background-color: #d93025;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}
.project-list {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}
.project-card {
  background-color: white;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.05);
  transition: transform 0.2s, box-shadow 0.2s;
}
.project-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
}
.project-card h2 {
  font-size: 1.25rem;
  margin-top: 0;
  margin-bottom: 0.5rem;
}
.project-card p {
  color: #666;
  font-size: 0.9rem;
  line-height: 1.5;
}
.loading-indicator, .no-projects {
  text-align: center;
  margin-top: 4rem;
  color: #888;
  font-size: 1.2rem;
}
.tasks-section {
  margin-top: 1.5rem;
  border-top: 1px solid #f0f0f0;
  padding-top: 1rem;
}

.tasks-section h4 {
  margin: 0 0 0.75rem 0;
  font-size: 0.9rem;
  font-weight: 600;
  color: #333;
}

/* 任务列表 <ul> 的样式 */
.tasks-section ul {
  list-style-type: none; /* 移除默认的圆点 */
  padding-left: 0;
  margin: 0;
}

/* 每个任务项 <li> 的样式 */
.tasks-section li {
  border-radius: 5px;
  margin-bottom: 6px;
  transition: background-color 0.2s ease-in-out;
}

/* 任务项内部链接 <a> 的样式 (RouterLink会被渲染成a标签) */
.tasks-section li a {
  display: block; /* 让整个区域都可以点击 */
  padding: 10px 12px;
  color: #202124; /* 改变链接颜色为深灰色 */
  text-decoration: none; /* 移除下划线 */
  transition: color 0.2s ease-in-out;
  border-radius: 5px;
}

/* 鼠标悬浮在任务项上时的效果 */
.tasks-section li:hover {
  background-color: #f1f3f4;
}

/* ▼▼▼ 这是我们为已完成任务添加的特殊样式 ▼▼▼ */
.tasks-section li.task-completed a {
  color: #9aa0a6; /* 已完成的任务文字变灰 */
  text-decoration: line-through; /* 添加删除线 */
}

/* (可选) 在已完成任务前添加一个对勾符号作为视觉提示 */
.tasks-section li.task-completed a::before {
  content: '✓';
  color: #1e8449; /* 绿色 */
  font-weight: bold;
  margin-right: 8px;
}


.no-tasks-message {
  font-style: italic;
  color: #999;
  font-size: 0.85rem;
  padding: 10px 0;
}

/* 在 HomeView.vue 的 <style scoped> 中添加 */
.add-task-form {
  display: flex;
  margin-top: 1rem;
  gap: 8px;
}
.add-task-form input {
  flex-grow: 1;
  border: 1px solid #ddd;
  border-radius: 4px;
  padding: 8px;
  font-size: 14px;
}
.add-task-form button {
  border: none;
  background-color: #28a745;
  color: white;
  font-weight: bold;
  border-radius: 4px;
  cursor: pointer;
  padding: 0 12px;
}
/* 在 HomeView.vue 的 <style scoped> 中添加 */

.tasks-section li {
  /* 使用Flexbox来对齐复选框和文字 */
  display: flex;
  align-items: center;
  background-color: #f9f9f9;
  border-radius: 5px;
  margin-bottom: 6px;
  transition: background-color 0.2s ease-in-out;
}

.task-checkbox {
  margin-right: 12px;
  /* 放大一点复选框，方便点击 */
  transform: scale(1.2);
  margin-left: 12px;
  cursor: pointer;
}
.add-task-button-container {
  margin-top: 1rem;
  text-align: right;
}
.add-task-button {
  background: none;
  border: 1px solid #1a73e8;
  color: #1a73e8;
  padding: 4px 10px;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 600;
}
.form-group {
  margin-bottom: 1rem;
}
.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
}
.form-group input, .form-group textarea {
  width: 100%;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 4px;
  box-sizing: border-box;
}
.submit-task-button {
  width: 100%;
  padding: 12px;
  font-size: 16px;
  color: white;
  background-color: #add8e6;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}
/* 在 HomeView.vue 的 <style scoped> 中修改和添加 */

.tasks-section li {
  display: flex;
  align-items: center;
  background-color: #f9f9f9;
  border-radius: 5px;
  margin-bottom: 6px;
  transition: background-color 0.2s ease-in-out;
  position: relative; /* 为删除按钮的定位做准备 */
}

/* 让链接占据大部分空间 */
.tasks-section li a {
  flex-grow: 1; /* 新增：让链接占据所有剩余空间 */
  display: block;
  padding: 10px 12px;
  color: #202124;
  text-decoration: none;
  transition: color 0.2s ease-in-out;
  border-radius: 5px;
}

/* 删除按钮的样式 */
.delete-task-btn {
  border: none;
  background: none;
  color: #aaa;
  font-size: 20px;
  font-weight: bold;
  cursor: pointer;
  padding: 0 12px;
  opacity: 0; /* 默认隐藏 */
  transition: opacity 0.2s ease-in-out;
}

/* 当鼠标悬浮在整个任务项上时，显示删除按钮 */
.tasks-section li:hover .delete-task-btn {
  opacity: 1;
}

.delete-task-btn:hover {
  color: #d93025; /* 鼠标悬浮在x上时变红 */
}
.create-project-btn {
  background-color: #1a73e8;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 600;
  margin-right: 1rem;
}

.project-card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.project-actions .action-btn {
  background: none;
  border: 1px solid #ccc;
  color: #555;
  padding: 4px 8px;
  font-size: 12px;
  border-radius: 4px;
  cursor: pointer;
  margin-left: 8px;
}

.project-actions .action-btn.delete:hover {
  background-color: #d93025;
  color: white;
  border-color: #d93025;
}
</style>