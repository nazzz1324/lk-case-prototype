// API Client for backend communication
// Use hardcoded fallback to ensure it works even if env is not loaded
const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5041/api';

interface ApiResponse<T> {
  isSuccess: boolean;
  data?: T;
  count?: number;
  errorMessage?: string;
  errorCode?: number;
}

class ApiClient {
  private baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  private async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    // Ensure no double slashes
    const cleanBaseUrl = this.baseUrl.endsWith('/') ? this.baseUrl.slice(0, -1) : this.baseUrl;
    const cleanEndpoint = endpoint.startsWith('/') ? endpoint : `/${endpoint}`;
    const url = `${cleanBaseUrl}${cleanEndpoint}`;
    
    const defaultHeaders: HeadersInit = {
      'Content-Type': 'application/json',
    };

    const config: RequestInit = {
      ...options,
      headers: {
        ...defaultHeaders,
        ...options.headers,
      },
    };

    console.log(`🚀 API Request: ${config.method || 'GET'} ${url}`, options.body ? JSON.parse(options.body as string) : '');

    try {
      const response = await fetch(url, config);
      
      if (!response.ok) {
        // Пытаемся прочитать тело ошибки, чтобы понять причину 400 Bad Request
        let errorDetail = '';
        try {
          const errorData = await response.json();
          errorDetail = JSON.stringify(errorData, null, 2);
          console.error(`❌ API Error Details (${response.status}):`, errorData);
        } catch (e) {
          errorDetail = await response.text();
          console.error(`❌ API Error Text (${response.status}):`, errorDetail);
        }
        throw new Error(`HTTP ${response.status}: ${errorDetail || response.statusText}`);
      }

      const data = await response.json();
      return data;
    } catch (error) {
      console.error(`❌ API request failed for ${endpoint}:`, error);
      throw error;
    }
  }

  // Auth endpoints
  async register(data: any) {
    return this.request<ApiResponse<any>>('/Auth/register', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }
  
  // User endpoints
  async getUsers() {
    return this.request<ApiResponse<any[]>>('/User/getUsers');
  }

  async getProfile() {
    const userId = sessionStorage.getItem("userId");
    return this.request<ApiResponse<any>>(`/User/getUsers?id=${userId}`);
  }

  async deleteUser(id: number) {
    return this.request<ApiResponse<boolean>>(`/User/deleteUser?id=${id}`, {
      method: 'DELETE',
    });
  }

  async updateUser(data: any) {
    return this.request<ApiResponse<boolean>>('/User/updateUser', {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  // Group endpoints
  async getGroups() {
    return this.request<ApiResponse<any[]>>('/Group/getGroups');
  }

  async createGroup(data: any) {
    return this.request<ApiResponse<any>>('/Group/createGroup', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async deleteGroup(id: number) {
    return this.request<ApiResponse<any>>(`/Group/deleteGroup?id=${id}`, {
      method: 'DELETE',
    });
  }

  async updateGroup(data: any) {
    return this.request<ApiResponse<any>>('/Group/updateGroup', {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  // Discipline endpoints
  async getDisciplines() {
    return this.request<ApiResponse<any[]>>('/Discipline/getDisciplines');
  }

  async createDiscipline(data: { name: string; index: string; indicatorIds: number[] }) {
    return this.request<ApiResponse<any>>('/Discipline/createDiscipline', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async updateDiscipline(data: { id: number; name: string; index: string; indicatorIds: number[] }) {
    return this.request<ApiResponse<boolean>>('/Discipline/updateDiscipline', {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  async deleteDiscipline(id: number) {
  return this.request<ApiResponse<boolean>>(`/Discipline/deleteDiscipline?id=${id}`, {
    method: 'DELETE',
  });
  }
  

  // Competence endpoints
  async getCompetences() {
    return this.request<ApiResponse<any[]>>('/Competence/getCompetences');
  }

  async createCompetence(data: any) {
    return this.request<ApiResponse<any>>('/Competence/createCompetence', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async deleteCompetence(id: number) {
    return this.request<ApiResponse<any>>(`/Competence/deleteCompetence?id=${id}`, {
      method: 'DELETE',
    });
  }

  async updateCompetence(data: any) {
    return this.request<ApiResponse<any>>('/Competence/updateCompetence', {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  // Indicator endpoints
  async getIndicators() {
    return this.request<ApiResponse<any[]>>('/Indicator/getIndicators');
  }

  async searchIndicators(search: string) {
    return this.request<ApiResponse<any[]>>(`/Indicator/searchIndicators?search=${encodeURIComponent(search)}`);
  }

  async createIndicator(data: any) {
    return this.request<ApiResponse<any>>('/Indicator/createIndicator', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async deleteIndicator(id: number) {
    return this.request<ApiResponse<any>>(`/Indicator/deleteIndicator?id=${id}`, {
      method: 'DELETE',
    });
  }

  async updateIndicator(data: any) {
    return this.request<ApiResponse<any>>('/Indicator/updateIndicator', {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  // Professional Role endpoints
  async getProfessionalRoles() {
    return this.request<ApiResponse<any[]>>('/ProfessionalRole/getProles');
  }

  async createProfessionalRole(data: any) {
    return this.request<ApiResponse<any>>('/ProfessionalRole/createProle', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async deleteProfessionalRole(id: number) {
    return this.request<ApiResponse<any>>(`/ProfessionalRole/deleteProle?id=${id}`, {
      method: 'DELETE',
    });
  }

  async updateProfessionalRole(data: any) {
    return this.request<ApiResponse<any>>('/ProfessionalRole/updateProle', {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  // Teacher endpoints
  async getTeacherDisciplines(teacherId: number) {
    return this.request<ApiResponse<any[]>>(`/Teacher/getDisciplines?teacherId=${teacherId}`);
  }

  async getScoringData(filter: { disciplineId: number; groupId: number; teacherId: number }) {
    const params = new URLSearchParams({
      disciplineId: filter.disciplineId.toString(),
      groupId: filter.groupId.toString(),
      teacherId: filter.teacherId.toString(),
    });
    return this.request<ApiResponse<any>>(`/Teacher/getScoringData?${params}`);
  }

  async saveScores(data: { disciplineId: number; teacherId: number; scores: any[] }) {
    return this.request<ApiResponse<boolean>>('/Teacher/saveScores', {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }
  // Student Endpoints
  async getStudentDisciplines(studentId: number) {
    return this.request<ApiResponse<any[]>>(`/Student/getDisciplines?studentId=${studentId}`);
  }
  async getStudentDisciplineScore(studentId: number, disciplineId: number) {
    return this.request<ApiResponse<any>>(`/Student/getDisciplineScores?studentId=${studentId}&disciplineId=${disciplineId}`);
  }
  // НОВЫЙ МЕТОД для компетенций
  async getStudentCompetences(studentId: number) {
    return this.request<ApiResponse<any[]>>(`/Student/getCompetences?studentId=${studentId}`);
  }

  async getStudentCompetenceScores(studentId: number, competenceId: number) {
  // Проверь правильный URL у бека
    return this.request<ApiResponse<any>>(`/Student/getCompetenceScores?studentId=${studentId}&competenceId=${competenceId}`);
  }
  // Профроли
  async getStudentProles(studentId: number) {
    return this.request<ApiResponse<any>>(`/Student/getStudentProles?studentId=${studentId}`);
  }
  
}

export const apiClient = new ApiClient(API_BASE_URL);
