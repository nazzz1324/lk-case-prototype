import { useState, useEffect } from "react";
import { Save } from "lucide-react";
import { Button } from "@/components/ui/button";
import { api } from "@/lib/api";
import { useParams } from "wouter";

interface GradeData {
  [studentId: number]: {
    [indicatorId: number]: {
      grade: number;
    };
  };
}

export default function Grading() {
  const { id: disciplineId } = useParams();
  const [groups, setGroups] = useState<any[]>([]);
  const [selectedGroup, setSelectedGroup] = useState("");
  const [indicators, setIndicators] = useState<any[]>([]);
  const [students, setStudents] = useState<any[]>([]);
  const [grades, setGrades] = useState<GradeData>({});
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [groupsData, scoringData] = await Promise.all([
          api.admin.getGroups(),
          api.teacher.getScoring({ disciplineId, groupId: selectedGroup }),
        ]);
        setGroups(groupsData);
        if (scoringData) {
          setIndicators(scoringData.indicators || []);
          setStudents(scoringData.students || []);
          // Инициализация оценок из scoringData
        }
      } catch (error) {
        console.error("Failed to fetch grading data:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, [disciplineId, selectedGroup]);

  const handleGradeChange = (studentId: number, indicatorId: number, value: number) => {
    setGrades((prev) => ({
      ...prev,
      [studentId]: {
        ...(prev[studentId] || {}),
        [indicatorId]: { grade: value },
      },
    }));
  };

  const getGrade = (studentId: number, indicatorId: number) => {
    return grades[studentId]?.[indicatorId]?.grade || 0;
  };

  const handleSave = async () => {
    try {
      await api.teacher.saveScores({
        disciplineId: parseInt(disciplineId!),
        scores: grades
      });
      alert("Оценки сохранены!");
    } catch (error) {
      console.error("Failed to save scores:", error);
    }
  };

  if (isLoading) return <div>Загрузка...</div>;

  return (
    <div className="space-y-6">
      <div className="bg-card rounded-2xl p-6 shadow-sm border border-border">
        <h3 className="text-lg font-semibold text-foreground mb-4">Выбор группы</h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div className="space-y-2">
            <label className="text-sm font-medium text-foreground">Группа</label>
            <select
              value={selectedGroup}
              onChange={(e) => setSelectedGroup(e.target.value)}
              className="w-full px-4 py-2 rounded-lg border border-border bg-background text-foreground"
            >
              <option value="">Выберите группу</option>
              {groups.map((group) => (
                <option key={group.id} value={group.id}>{group.name}</option>
              ))}
            </select>
          </div>
        </div>
      </div>

      <div className="bg-card rounded-2xl shadow-sm border border-border overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-border bg-secondary">
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground sticky left-0 bg-secondary z-10">Студент</th>
                {indicators.map((indicator) => (
                  <th key={indicator.id} className="px-4 py-4 text-center text-sm font-semibold text-foreground whitespace-nowrap">
                    <div className="text-xs text-primary font-medium">{indicator.code}</div>
                    <div className="text-xs text-muted-foreground">{indicator.name.substring(0, 20)}...</div>
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {students.map((student) => (
                <tr key={student.id} className="border-b border-border hover:bg-secondary transition-colors">
                  <td className="px-6 py-4 text-sm font-medium text-foreground sticky left-0 bg-card hover:bg-secondary z-10">{student.name}</td>
                  {indicators.map((indicator) => (
                    <td key={indicator.id} className="px-4 py-4 text-center border-l border-border">
                      <input
                        type="number"
                        min="0"
                        max="100"
                        value={getGrade(student.id, indicator.id)}
                        onChange={(e) => handleGradeChange(student.id, indicator.id, parseInt(e.target.value) || 0)}
                        className="w-12 px-2 py-1 text-center border border-border rounded bg-background text-sm"
                      />
                    </td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <div className="flex gap-4 justify-end">
        <Button onClick={handleSave} className="bg-primary text-primary-foreground">
          <Save size={18} className="mr-2" /> Сохранить изменения
        </Button>
      </div>
    </div>
  );
}
