import React from "react";

const AddPostInputField = ({ title, description, address, volunteerArea, jobType, initialDate, lastDate, onChange }) => {
  return (
    <div className="add-post-input-fields">
      <div className="add-post-form-group">
        <label>Title:</label>
        <input
          type="text"
          name="title"
          value={title}
          onChange={onChange}
          required
        />
      </div>

      <div className="add-post-form-group">
        <label>Description:</label>
        <textarea
          name="description"
          value={description}
          onChange={onChange}
          required
          rows="10"
          style={{ height: '230px' }}
        />
      </div>

      <div className="add-post-form-group">
        <label>Address:</label>
        <input
          type="text"
          name="address"
          value={address}
          onChange={onChange}
          required
        />
      </div>

      <div className="add-post-form-group">
        <label>Area:</label>
        <select name="volunteerArea" value={volunteerArea} onChange={onChange}>
          <option value="">Select Area</option>
          <option value="North">North</option>
          <option value="South">South</option>
          <option value="Central">Central</option>
        </select>
      </div>

      <div className="add-post-form-group">
        <label>Job Type:</label>
        <select name="jobType" value={jobType} onChange={onChange}>
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
          onChange={onChange}
        />
      </div>

      <div className="add-post-form-group">
        <label>End Date:</label>
        <input
          type="date"
          name="lastDate"
          value={lastDate}
          onChange={onChange}
        />
      </div>
    </div>
  );
};

export default AddPostInputField;
