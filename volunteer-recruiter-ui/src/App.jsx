import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Auth from './components/login-signup/Auth';
import HomePage from './components/mainPage/HomePage';
import MyPostsPage from './components/myPosts/MyPostsPage';
import AddPost from './components/myPosts/addPost/AddPostPage';

/**
 * `App` is the main application component that sets up routing for different pages.
 * 
 * Routes:
 * - `/`: Renders the `Auth` component for login/signup.
 * - `/main`: Renders the `HomePage` component displaying available posts.
 * - `/my-posts`: Renders the `MyPostsPage` component for viewing user's posts.
 * - `/add-post`: Renders the `AddPost` component for creating or editing posts.
 * 
 * Usage:
 * ```jsx
 * import App from './App';
 * ```
 */
function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Auth />} />
        <Route path="/main" element={<HomePage />} />
        <Route path="/my-posts" element={<MyPostsPage />} />
        <Route path="/add-post" element={<AddPost />} />
      </Routes>
    </Router>

  );
}

export default App;