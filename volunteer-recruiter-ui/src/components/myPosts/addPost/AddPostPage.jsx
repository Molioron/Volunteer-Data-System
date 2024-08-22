import { json, useNavigate } from 'react-router-dom';
import React, { useState } from 'react';
import AddPostInputField from './AddPostInputField';
import './AddPostPage.css';
const AddPostPage = () => {
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        connectionKey: '',
        title: '',
        description: '',
        address: '',
        volunteerArea: '',
        jobType: '',
        initialDate: '',
        lastDate: ''
    });

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        const url = 'http://localhost:9000/createpost';
        const connectionKey = document.cookie.split('=')[1];
        const updateformData = { ...formData, connectionKey: connectionKey };

        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(updateformData)
            });
            console.log(JSON.stringify(updateformData));
            const result = await response.json();
            handleResponse(result, 'add-post');
        } catch (error) {
            console.error('Error:', error);
            alert('Error: ' + error.message);
        }
    }

    const handleResponse = (response) => {
        try {
            const operationStatus = JSON.parse(response.operationStatus);
            if (operationStatus.Code === 'Success') {
                alert(operationStatus.Message);
                navigate('/my-posts');
            } else {
                alert('Error1: ' + operationStatus.Message);
            }
        } catch (error) {
            console.error('Error2:', error);
            alert('Error2: ' + error.message);
        }

    }


    const handleLogout = async () => {
        try {
          const connectionKey = document.cookie.split('=')[1];
          const response = await fetch('http://localhost:9000/logout', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
            },
            body: JSON.stringify({ connectionKey: connectionKey }),
          });
    
          const result = await response.json();
          const operationStatus = JSON.parse(result.operationStatus);
          if (operationStatus.Code === 'Success') {
            alert(operationStatus.Message);
            document.cookie = "connectionKey=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
            navigate('/my-posts');
          } else {
            alert('Error: ' + result.operationStatus.Message);
          }
        } catch (error) {
          console.error('Error:', error);
          alert('Error: ' + error.message);
        }
      };
    
    
    
    const handleHome = () => {
        navigate('/main');
    }


    return(
        <div className="add-post-page">
            <button className="logout-button" onClick={handleLogout}>Logout</button>
            <div>
                <div className='add-post'>
                <AddPostInputField
                    title = {formData.title}
                    description={formData.description}
                    address={formData.address}
                    volunteerArea={formData.volunteerArea}
                    jobType={formData.jobType}
                    initialDate={formData.initialDate}
                    lastDate={formData.lastDate}
                    onChange={handleInputChange}    
                    />
                </div>
                
            </div>
            <div className="add-post-action-button">
                <button onClick={handleHome}>Home</button>
                <button onClick={handleSubmit}>Submit</button>
            </div>
        </div>
    );

};
export default AddPostPage;