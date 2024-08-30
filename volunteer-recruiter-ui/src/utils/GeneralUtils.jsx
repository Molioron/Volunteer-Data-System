import { BASE_URL } from "./ApiUtils";

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

  export const formatDate = (dateString) => {
    let [year, month, day] = dateString.split('-');
    day = day.split('T')[0];
    day = day.length === 1 ? `0${day}` : day;
    return `${day}.${month}.${year}`;
  };

  export const PostItem = ({ post }) => (
    <div key={post.Id} className="post-item">
      <h3>{post.Title}</h3>
      <p><strong>Description:</strong> {post.Description}</p>
      <p><strong>Volunteer Area:</strong> {post.Location}</p>
      <p><strong>Job:</strong> {post.Job}</p>
      <p><strong>Address:</strong> {post.Address}</p>
      <p><strong>Phone Number:</strong> {post.PhoneNumber}</p>
      <p><strong>Dates:</strong> {formatDate(post.InitialDate)} - {formatDate(post.LastDate)}</p>
    </div>
  );

  