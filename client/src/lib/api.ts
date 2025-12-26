
const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

async function request<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
  const token = sessionStorage.getItem("token");
  const headers = {
    "Content-Type": "application/json",
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...options.headers,
  };

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    ...options,
    headers,
  });

  const result = await response.json();

  if (!response.ok || !result.isSuccess) {
    throw new Error(result.message || "Request failed");
  }

  return result.data;
}

export const api = {
  auth: {
    login: (data: any) => request<any>("/Auth/login", {
      method: "POST",
      body: JSON.stringify(data),
    }),
    register: (data: any) => request<any>("/Auth/register", {
      method: "POST",
      body: JSON.stringify(data),
    }),
  },
  admin: {
    getUsers: () => request<any[]>("/Student"), // Assuming Student controller for users or similar
    getGroups: () => request<any[]>("/Group"),
    getDisciplines: () => request<any[]>("/Discipline"),
    getCompetencies: () => request<any[]>("/Competence"),
    getIndicators: () => request<any[]>("/Indicator"),
    getProfessions: () => request<any[]>("/ProfessionalRole"),
  },
  teacher: {
    getDisciplines: (teacherId: number) => request<any[]>(`/Teacher/disciplines/${teacherId}`),
    getScoring: (params: any) => {
      const query = new URLSearchParams(params).toString();
      return request<any>(`/Teacher/scoring?${query}`);
    },
    saveScores: (data: any) => request<any>("/Teacher/scores", {
      method: "PUT",
      body: JSON.stringify(data),
    }),
  },
  student: {
    getProfile: (studentId: number) => request<any>(`/Student/${studentId}`),
    getDisciplines: (studentId: number) => request<any[]>(`/Student/disciplines/${studentId}`),
    getCompetencies: (studentId: number) => request<any[]>(`/Student/competences/${studentId}`),
    getScores: (studentId: number, disciplineId: number) => request<any>(`/Student/scores/${studentId}/${disciplineId}`),
  }
};
