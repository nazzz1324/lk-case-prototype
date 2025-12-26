import { useState, useEffect } from "react";
import { Search, ArrowRight } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { apiClient } from "@/lib/api";
import { useLocation } from "wouter";
import { toast } from "sonner";

export default function TeacherDisciplines() {
  const [, setLocation] = useLocation();
  const [disciplines, setDisciplines] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [error, setError] = useState<string | null>(null);

  const teacherId = parseInt(sessionStorage.getItem("userId") || "0");

  useEffect(() => {
    if (teacherId === 0) {
      setError("Пользователь не авторизован. Пожалуйста, войдите в систему.");
      setLoading(false);
      return;
    }
    fetchDisciplines();
  }, [teacherId]);

  const fetchDisciplines = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await apiClient.getTeacherDisciplines(teacherId);
      if (response.data) {
        setDisciplines(response.data);
      }
    } catch (error: any) {
      console.error("Failed to fetch disciplines:", error);
      if (error.message?.includes("404") || error.message?.includes("TeacherNotFound")) {
        setError("Преподаватель не найден в системе. Обратитесь к администратору.");
      } else {
        setError("Не удалось загрузить дисциплины. Попробуйте обновить страницу.");
      }
      toast.error("Не удалось загрузить дисциплины");
    } finally {
      setLoading(false);
    }
  };

  const filteredDisciplines = disciplines.filter((discipline) => {
    return discipline.name.toLowerCase().includes(searchTerm.toLowerCase());
  });

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-lg text-muted-foreground">Загрузка...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-center space-y-4">
          <div className="text-lg text-red-600">{error}</div>
          <Button onClick={() => setLocation("/login")} className="bg-primary hover:bg-primary/90">
            Перейти к входу
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header with Search */}
      <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
          <Input
            placeholder="Поиск по названию дисциплины..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10"
          />
        </div>
      </div>

      {/* Disciplines Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {filteredDisciplines.map((discipline) => (
          <div
            key={discipline.id}
            className="bg-card rounded-2xl p-6 shadow-sm border border-border hover:shadow-md hover:border-primary transition-all group"
          >
            <div className="space-y-4">
              {/* Header */}
              <div>
                <h3 className="text-lg font-semibold text-foreground mb-1">{discipline.name}</h3>
              </div>

              {/* Stats */}
              <div className="bg-secondary rounded-lg p-3">
                <p className="text-xs text-muted-foreground">Индикаторов</p>
                <p className="text-2xl font-bold text-foreground">{discipline.indicatorCount || 0}</p>
              </div>

              {/* Progress Bar */}
              <div>
                <div className="flex justify-between items-center mb-2">
                  <p className="text-xs text-muted-foreground">Заполнение оценок</p>
                  <p className="text-xs font-semibold text-primary">—</p>
                </div>
                <div className="w-full bg-secondary rounded-full h-2">
                  <div
                    className="bg-gradient-to-r from-primary to-sidebar-primary h-2 rounded-full"
                    style={{ width: "0%" }}
                  />
                </div>
              </div>

              {/* Action Button */}
              <Button
                onClick={() => setLocation(`/teacher/grading/${discipline.id}`)}
                className="w-full bg-primary hover:bg-primary/90 text-primary-foreground group-hover:translate-x-1 transition-transform"
              >
                Перейти к оцениванию
                <ArrowRight size={16} className="ml-2" />
              </Button>
            </div>
          </div>
        ))}
      </div>

      {filteredDisciplines.length === 0 && (
        <div className="text-center py-12">
          <p className="text-muted-foreground">Дисциплины не найдены</p>
        </div>
      )}
    </div>
  );
}
