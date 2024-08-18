import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Auth from './components/login-signup/Auth';
import HomePage from './components/mainPage/HomePage';
import MyPostsPage from './components/myPosts/MyPostsPage';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Auth />} />
        <Route path="/main" element={<HomePage />} />
        <Route path="/my-posts" element={<MyPostsPage />} />
      </Routes>
    </Router>

  );
}

export default App;
{/* <div className="App">
<Auth />
</div> */}