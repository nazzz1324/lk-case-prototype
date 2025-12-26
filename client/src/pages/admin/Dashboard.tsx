import { useState, useEffect } from "react";
import { Users, BookOpen, Target, Award, GraduationCap } from "lucide-react";
import MetricCard from "@/components/MetricCard";
import { apiClient } from "@/lib/api";
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, PieChart, Pie, Cell } from "recharts";

// const disciplineData = [
//   { name: "Веб-разработка", competencies: 5 },
//   { name: "Базы данных", competencies: 4 },
//   { name: "Алгоритмы", competencies: 6 },
//   { name: "ООП", competencies: 5 },
//   { name: "Сети", competencies: 3 },
// ];

// const activityData = [
//   { name: "Пн", оценки: 12 },
//   { name: "Вт", оценки: 19 },
//   { name: "Ср", оценки: 15 },
//   { name: "Чт", оценки: 25 },
//   { name: "Пт", оценки: 22 },
//   { name: "Сб", оценки: 8 },
//   { name: "Вс", оценки: 5 },
// ];

// const competencyDistribution = [
//   { name: "ПК-1", value: 25 },
//   { name: "ПК-2", value: 20 },
//   { name: "ПК-3", value: 30 },
//   { name: "ОК-1", value: 25 },
// ];

const COLORS = ["#377DFF", "#A6C8FF", "#2D4DBF", "#5B9EFF"];

export default function Dashboard() {
  const [metrics, setMetrics] = useState({
    users: 0,
    disciplines: 0,
    competencies: 0,
    indicators: 0,
    groups: 0,
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    try {
      setLoading(true);
      const [usersRes, disciplinesRes, competenciesRes, indicatorsRes, groupsRes] = await Promise.all([
        apiClient.getUsers().catch(() => ({ data: [] })),
        apiClient.getDisciplines().catch(() => ({ data: [] })),
        apiClient.getCompetences().catch(() => ({ data: [] })),
        apiClient.getIndicators().catch(() => ({ data: [] })),
        apiClient.getGroups().catch(() => ({ data: [] })),
      ]);

      setMetrics({
        users: usersRes.data?.length || 0,
        disciplines: disciplinesRes.data?.length || 0,
        competencies: competenciesRes.data?.length || 0,
        indicators: indicatorsRes.data?.length || 0,
        groups: groupsRes.data?.length || 0,
      });
    } catch (error) {
      console.error("Failed to fetch dashboard data:", error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-lg text-muted-foreground">Загрузка...</div>
      </div>
    );
  }

  return (
    <div className="space-y-8">
      {/* Metrics Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-4">
        <MetricCard
          icon={<Users size={24} />}
          value={metrics.users}
          label="Пользователей"
          color="blue"
        />
        <MetricCard
          icon={<BookOpen size={24} />}
          value={metrics.disciplines}
          label="Дисциплин"
          color="purple"
        />
        <MetricCard
          icon={<Target size={24} />}
          value={metrics.competencies}
          label="Компетенций"
          color="green"
        />
        <MetricCard
          icon={<Award size={24} />}
          value={metrics.indicators}
          label="Индикаторов"
          color="orange"
        />
        <MetricCard
          icon={<GraduationCap size={24} />}
          value={metrics.groups}
          label="Групп"
          color="red"
        />
      </div>
      {/* 
      Charts Grid
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Discipline Competencies Chart 
        <div className="bg-card rounded-2xl shadow-sm border border-border p-6">
          <h3 className="text-lg font-semibold mb-4 text-foreground">Компетенции по дисциплинам</h3>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={disciplineData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="name" />
              <YAxis />
              <Tooltip />
              <Legend />
              <Bar dataKey="competencies" fill="#377DFF" />
            </BarChart>
          </ResponsiveContainer>
        </div>

        {/* Activity Chart 
        <div className="bg-card rounded-2xl shadow-sm border border-border p-6">
          <h3 className="text-lg font-semibold mb-4 text-foreground">Активность за неделю</h3>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={activityData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="name" />
              <YAxis />
              <Tooltip />
              <Legend />
              <Bar dataKey="оценки" fill="#A6C8FF" />
            </BarChart>
          </ResponsiveContainer>
        </div>

        {/* Competency Distribution 
        <div className="bg-card rounded-2xl shadow-sm border border-border p-6">
          <h3 className="text-lg font-semibold mb-4 text-foreground">Распределение компетенций</h3>
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={competencyDistribution}
                cx="50%"
                cy="50%"
                labelLine={false}
                label={(entry) => entry.name}
                outerRadius={80}
                fill="#8884d8"
                dataKey="value"
              >
                {competencyDistribution.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                ))}
              </Pie>
              <Tooltip />
            </PieChart>
          </ResponsiveContainer>
        </div>

        {/* Recent Activity 
        <div className="bg-card rounded-2xl shadow-sm border border-border p-6">
          <h3 className="text-lg font-semibold mb-4 text-foreground">Последние действия</h3>
          <div className="space-y-4">
            <div className="flex items-start gap-3">
              <div className="w-2 h-2 rounded-full bg-blue-500 mt-2"></div>
              <div>
                <p className="text-sm font-medium text-foreground">Добавлена новая дисциплина</p>
                <p className="text-xs text-muted-foreground">2 часа назад</p>
              </div>
            </div>
            <div className="flex items-start gap-3">
              <div className="w-2 h-2 rounded-full bg-green-500 mt-2"></div>
              <div>
                <p className="text-sm font-medium text-foreground">Обновлены компетенции</p>
                <p className="text-xs text-muted-foreground">5 часов назад</p>
              </div>
            </div>
            <div className="flex items-start gap-3">
              <div className="w-2 h-2 rounded-full bg-purple-500 mt-2"></div>
              <div>
                <p className="text-sm font-medium text-foreground">Создан новый пользователь</p>
                <p className="text-xs text-muted-foreground">1 день назад</p>
              </div>
            </div>
            <div className="flex items-start gap-3">
              <div className="w-2 h-2 rounded-full bg-orange-500 mt-2"></div>
              <div>
                <p className="text-sm font-medium text-foreground">Добавлены индикаторы</p>
                <p className="text-xs text-muted-foreground">2 дня назад</p>
              </div>
            </div>
          </div>
        </div>
      </div>*/}
    </div>
  );
}
