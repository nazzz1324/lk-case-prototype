// Type definitions for the application

export interface User {
  id: number;
  firstname: string;
  lastname: string;
  middlename: string;
  email: string;
  role: string;
  roleId: number;
  isActive: boolean;
  group?: string | null;
}

export interface Group {
  id: number;
  name: string;
  course: number;
  studentCount?: number;
}

export interface Discipline {
  id: number;
  name: string;
  code: string;
  hours: number;
  credits: number;
}

export interface Competence {
  id: number;
  code: string;
  name: string;
  description?: string;
}

export interface Indicator {
  id: number;
  code: string;
  name: string;
  description?: string;
  competenceId?: number;
}

export interface ProfessionalRole {
  id: number;
  name: string;
  description?: string;
}

export interface Grade {
  id: number;
  studentId: number;
  disciplineId: number;
  indicatorId: number;
  value: number;
  date: string;
}

export interface DashboardMetric {
  title: string;
  value: string | number;
  change: string;
  icon: string;
}
