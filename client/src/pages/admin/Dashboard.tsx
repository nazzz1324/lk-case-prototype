import { Users, BookOpen, Target, Pin, Users2 } from "lucide-react";
import { useState, useEffect } from "react";
import MetricCard from "@/components/MetricCard";
import { api } from "@/lib/api";

export default function Dashboard() {
  const [metrics, setMetrics] = useState({
    users: 0,
    disciplines: 0,
    competencies: 0,
    indicators: 0,
    groups: 0,
  });
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchMetrics = async () => {
      try {
        // В реальном API может быть отдельный эндпоинт для метрик,
        // но если его нет, мы можем получить данные из списков.
        const [users, disciplines, competencies, indicators, groups] = await Promise.all([
          api.admin.getUsers().catch(() => []),
          api.admin.getDisciplines().catch(() => []),
          api.admin.getCompetencies().catch(() => []),
          api.admin.getIndicators().catch(() => []),
          api.admin.getGroups().catch(() => []),
        ]);

        setMetrics({
          users: users.length,
          disciplines: disciplines.length,
          competencies: competencies.length,
          indicators: indicators.length,
          groups: groups.length,
        });
      } catch (error) {
        console.error("Failed to fetch metrics:", error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchMetrics();
  }, []);

  if (isLoading) {
    return <div className="flex items-center justify-center h-64">Загрузка данных...</div>;
  }

  return (
    <div className="space-y-8">
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
          icon={<Pin size={24} />}
          value={metrics.indicators}
          label="Индикаторов"
          color="orange"
        />
        <MetricCard
          icon={<Users2 size={24} />}
          value={metrics.groups}
          label="Групп"
          color="blue"
        />
      </div>
    </div>
  );
}
