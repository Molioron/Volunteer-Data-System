import React from "react";

const AddPostInputField = ({ volunteerArea, jobType, initialDate, lastDate, onChange}) => {
    return (
      <div className="add-post-input-fields">

      <div className="add-post-form-group">
        <label>Title:</label>
        <input
        type="text"
        name="title"
        onChange={onChange}
        required
        />
      </div>

      <div className="add-post-form-group">
      <label>Description:</label>
      <textarea
        name="description"
        onChange={onChange}
        required
        rows="10" // Increase the number of rows
        style={{ height: '230px' }} // Set the height to 300px
        />
      </div>

      <div className="add-post-form-group">
        <label>Address:</label>
        <input
        type="text"
        name="address"
        onChange={onChange}
        required
        />
      </div>

      <div className="add-post-form-group">
        <label>Area:</label>
        <select name="volunteerArea" value={volunteerArea} onChange={(e) => onChange(e)}>
          <option value="">Select Area</option>
          <option value="North">North</option>
          <option value="South">South</option>
          <option value="Central">Central</option>
        </select>
      </div>

      <div className="add-post-form-group">
        <label>Job Type:</label>
        <select name="jobType" value={jobType} onChange={(e) => onChange(e)}>
          <option value="">Select Job Type</option>
          <option value="Agriculture">Agriculture</option>
          <option value="Cooking">Cooking</option>
          <option value="Transportation">Transportation</option>
          <option value="AnimalCare">Animal Care</option>
        </select>
      </div>

      <div className="add-post-form-group">
        <label>Initial Date:</label>
        <input
          type="date"
          name="initialDate"
          value={initialDate}
          onChange={(e) => onChange(e)}
        />
      </div>

      <div className="add-post-form-group">
        <label>End Date:</label>
        <input
          type="date"
          name="lastDate"
          value={lastDate}
          onChange={(e) => onChange(e)}
        />
      </div>

      </div>      
    );
  };
  
  export default AddPostInputField;