import React from 'react';
import { BASE_URL } from "./ApiUtils";
import Button from '../components/common/Button';


/**
 * `handleLogout` logs out the user by sending a request to the server and clearing the session cookie.
 * @param {Function} navigate - Function to navigate to the login page after logout.
 */
export const handleLogout = async (navigate) => {
    try {
      const connectionKey = document.cookie.split('=')[1];
      const response = await fetch(BASE_URL + '/logout', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ connectionKey: connectionKey }),
      });
  
      const result = await response.json();
      const operationStatus = JSON.parse(result.operationStatus);
  
      if (operationStatus.Code === 'Success') {
        alert('Logged out successfully');
        document.cookie = "connectionKey=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        navigate('/');
      } else {
        alert('Error: ' + result.operationStatus.Message);
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Error: ' + error.message);
    }
  };

/**
 * Formats a date string from 'YYYY-MM-DD' to 'DD.MM.YYYY'.
 * @param {string} dateString - Date string to format.
 * @returns {string} - Formatted date string.
 */
  export const formatDate = (dateString) => {
    let [year, month, day] = dateString.split('-');
    day = day.split('T')[0];
    day = day.length === 1 ? `0${day}` : day;
    return `${day}.${month}.${year}`;
  };

  
/**
 * `PostItem` is a React component displaying post details with options to join or leave.
 * 
 * Props:
 * - `post` (object): Post data.
 * - `handleJoin` (function): Function to join the post.
 * - `handleLeave` (function): Function to leave the post.
 */
  export const PostItem = ({ post, handleJoin, handleLeave }) => (
    <div key={post.Id} className="post-item">
      <h3>{post.Title}</h3>
      <p><strong>Description:</strong> {post.Description}</p>
      <p><strong>Volunteer Area:</strong> {post.Location}</p>
      <p><strong>Job:</strong> {post.Job}</p>
      <p><strong>Address:</strong> {post.Address}</p>
      <p><strong>Phone Number:</strong> {post.PhoneNumber}</p>
      <p><strong>Dates:</strong> {formatDate(post.InitialDate)} - {formatDate(post.LastDate)}</p>
  
      <Button className="join-button" onClick={() => handleJoin(post.Id)}>Join</Button>
      <Button className="leave-button" onClick={() => handleLeave(post.Id)}>Leave</Button>
    </div>
  );

  