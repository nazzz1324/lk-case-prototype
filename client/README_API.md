# Student Cabinet - API Integration

## Обзор изменений

Mock файл был удален, и фронтенд теперь работает с реальным API бэкенда.

## Структура API клиента

### Файлы
- `src/lib/api.ts` - Основной API клиент для взаимодействия с бэкендом
- `src/types/index.ts` - TypeScript интерфейсы для типизации данных
- `client/.env` - Конфигурация переменных окружения

### API Endpoints

#### Users (Пользователи)
- `GET /api/User/getUsers` - Получить всех пользователей
- `DELETE /api/User/deleteUser?id={id}` - Удалить пользователя
- `PUT /api/User/updateUser` - Обновить пользователя

#### Groups (Группы)
- `GET /api/Group/getGroups` - Получить все группы
- `POST /api/Group/createGroup` - Создать группу
- `DELETE /api/Group/deleteGroup?id={id}` - Удалить группу
- `PUT /api/Group/updateGroup` - Обновить группу

#### Disciplines (Дисциплины)
- `GET /api/Discipline/getDisciplines` - Получить все дисциплины
- `POST /api/Discipline/createDiscipline` - Создать дисциплину
- `DELETE /api/Discipline/deleteDiscipline?id={id}` - Удалить дисциплину
- `PUT /api/Discipline/updateDiscipline` - Обновить дисциплину

#### Competences (Компетенции)
- `GET /api/Competence/getCompetences` - Получить все компетенции
- `POST /api/Competence/createCompetence` - Создать компетенцию
- `DELETE /api/Competence/deleteCompetence?id={id}` - Удалить компетенцию
- `PUT /api/Competence/updateCompetence` - Обновить компетенцию

#### Indicators (Индикаторы)
- `GET /api/Indicator/getIndicators` - Получить все индикаторы
- `GET /api/Indicator/searchIndicators?search={query}` - Поиск индикаторов
- `POST /api/Indicator/createIndicator` - Создать индикатор
- `DELETE /api/Indicator/deleteIndicator?id={id}` - Удалить индикатор
- `PUT /api/Indicator/updateIndicator` - Обновить индикатор

#### Professional Roles (Профессии)
- `GET /api/ProfessionalRole/getProles` - Получить все профессии
- `POST /api/ProfessionalRole/createProle` - Создать профессию
- `DELETE /api/ProfessionalRole/deleteProle?id={id}` - Удалить профессию
- `PUT /api/ProfessionalRole/updateProle` - Обновить профессию

## Обновленные страницы

### Администратор
- ✅ `pages/admin/Users.tsx` - Управление пользователями
- ✅ `pages/admin/Groups.tsx` - Управление группами
- ✅ `pages/admin/Disciplines.tsx` - Управление дисциплинами
- ✅ `pages/admin/Competencies.tsx` - Управление компетенциями
- ✅ `pages/admin/Indicators.tsx` - Управление индикаторами
- ✅ `pages/admin/Professions.tsx` - Управление профессиями
- ✅ `pages/admin/Dashboard.tsx` - Панель управления с метриками

## Конфигурация

### Переменные окружения

Создайте файл `client/.env` со следующим содержимым:

```env
VITE_API_URL=http://localhost:5000/api
```

Измените URL на адрес вашего бэкенд сервера.

## Запуск проекта

### Backend (C# ASP.NET)
```bash
cd Account.api
dotnet run
```

### Frontend (React + Vite)
```bash
cd client
pnpm install
pnpm dev
```

## Особенности реализации

1. **Обработка ошибок**: Все API вызовы обернуты в try-catch блоки с уведомлениями через toast
2. **Загрузка данных**: Добавлены состояния загрузки (loading states) для лучшего UX
3. **Типизация**: Используются TypeScript интерфейсы для всех данных
4. **Централизованный API клиент**: Все запросы проходят через единый класс ApiClient
5. **Автоматическое обновление**: После создания/обновления/удаления данные автоматически перезагружаются

## Примечания

- Некоторые эндпоинты могут требовать дополнительной настройки на бэкенде
- Убедитесь, что CORS настроен правильно на бэкенде для работы с фронтендом
- Для production окружения измените VITE_API_URL на production URL
