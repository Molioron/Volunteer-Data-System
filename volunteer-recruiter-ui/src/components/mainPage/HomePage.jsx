import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import HomeInputFields from './HomeInputFields';
import './HomePage.css';
import Button from '../common/Button';
import { handleLogout, PostItem } from '../../utils/GeneralUtils';
import useFormInput from '../../hooks/UseFormInput';
import { apiRequest, BASE_URL } from '../../utils/ApiUtils';
import { fetchUserPosts } from '../../utils/NavigationUtils';

/**
 * `HomePage` displays and filters posts based on user-selected criteria.
 * 
 * Uses:
 * - `useNavigate` (hook): To navigate to different routes.
 * - `useLocation` (hook): To retrieve posts passed from previous route.
 * - `apiRequest` (function): To communicate with backend for fetching and joining/leaving posts.
 * 
 * State:
 * - `posts` (array): Holds the list of posts for display.
 * 
 * Functions:
 * - `fetchFilteredPosts`: Fetches filtered posts based on selected criteria.
 * - `handleJoin`: Joins a specified post.
 * - `handleLeave`: Leaves a specified post.
 */


const HomePage = () => {

  // Get location object to access state passed from previous route
  const location = useLocation();

  // Custom hook to manage form state; initialize with default values
  const { formData, handleInputChange} = useFormInput({
    volunteerAreas: [],
    jobTypes: [],
    initialDate: '',
    lastDate: '',
    dateFilterType: '',
  });


  // State to hold the posts; initialized with posts passed via location state or an empty array
  const [posts, setPosts] = useState(location.state?.posts || []); 

  const navigate = useNavigate();

  // Effect hook to update posts state if posts are passed via location state
  useEffect(() => {
    if (location.state?.posts) {
      setPosts(location.state.posts);
    }
  }, [location.state?.posts]);

  /**
   * Fetches posts that match the selected filter criteria.
   */

  const fetchFilteredPosts = async () => {
    try {
      const connectionKey = document.cookie.split('=')[1]; // Get the connection key from cookies
  
      const requestData = {
        ...formData,
        connectionKey: connectionKey, // Add connection key to the form data
      };
  
      const result = await apiRequest(BASE_URL + '/getfilteredposts', 'POST', requestData);
  
      const posts = JSON.parse(result.posts);
      setPosts(posts);
    } catch (error) {
      alert('Error: ' + error.message);
    }
  };

  /**
   * Sends a request to join a specified post.
   * @param {string} postId - ID of the post to join.
   */
  const handleJoin = async (postId) => {
    try {
      const result = await apiRequest(BASE_URL + '/joinpost', 'POST', {
        connectionKey: document.cookie.split('=')[1], 
        id: postId, 
      });
      const operationStatus = JSON.parse(result.operationStatus);
      if (operationStatus.Code !== 'Success') {
        alert('Error: ' + operationStatus.Message);
      } else {
        alert('Joined post successfully');
      }
    } catch (error) {
      alert('Error: ' + error.message);
    }
  };

  /**
   * Sends a request to leave a specified post.
   * @param {string} postId - ID of the post to leave.
   */
  const handleLeave = async (postId) => {
    try {
      const result = await apiRequest(BASE_URL + '/leavepost', 'POST', {
        connectionKey: document.cookie.split('=')[1], 
        id: postId, 
      });
      const operationStatus = JSON.parse(result.operationStatus);
      if (operationStatus.Code !== 'Success') {
        alert('Error: ' + operationStatus.Message);
      } else {
        alert('Left post successfully');
      }
    } catch (error) {
      alert('Error: ' + error.message);
    }
  };

  return (
    <div className="home-page">
      <Button className="logout-button" onClick={() => handleLogout(navigate)}>Logout</Button>
      <div className="input-fields-container">
        <HomeInputFields
          area={formData.volunteerAreas}
          jobTitle={formData.jobTypes}
          initialDate={formData.initialDate}
          lastDate={formData.lastDate}
          dateType={formData.dateFilterType}
          onChange={handleInputChange}
        />
        <div className="action-buttons">
          <Button onClick={fetchFilteredPosts}>Search</Button>
          <Button onClick={() => fetchUserPosts(navigate)}>My Posts</Button>
        </div>
      </div>
      <div className="content-container">
        <h2>Posts</h2>
        {posts.length > 0 ? (
          posts.map(post => (
            <PostItem 
              key={post.Id} 
              post={post}
              handleJoin={handleJoin}   
              handleLeave={handleLeave} 
            />
          ))
        ) : (
          <p>No posts to display.</p>
        )}
      </div>
    </div>
  );
};

export default HomePage;
