import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Auth from './components/login-signup/Auth';
import HomePage from './components/mainPage/HomePage';
import MyPostsPage from './components/myPosts/MyPostsPage';
import AddPost from './components/myPosts/addPost/AddPostPage';

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