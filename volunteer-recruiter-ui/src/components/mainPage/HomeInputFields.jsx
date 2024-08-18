import React from 'react';

const HomeInputFields = ({ area, jobTitle, initialDate, endDate, dateType, onChange }) => {
  return (
    <div className="input-fields">
      <div className="form-group">
        <label>Area:</label>
        <select name="area" value={area} onChange={(e) => onChange(e)}>
          <option value="">Select Area</option>
          <option value="area1">Area 1</option>
          <option value="area2">Area 2</option>
          <option value="area3">Area 3</option>
        </select>
      </div>

      <div className="form-group">
        <label>Job Title:</label>
        <select name="jobTitle" value={jobTitle} onChange={(e) => onChange(e)}>
          <option value="">Select Job Title</option>
          <option value="developer">Developer</option>
          <option value="designer">Designer</option>
          <option value="manager">Manager</option>
        </select>
      </div>

      <div className="form-group">
        <label>Initial Date:</label>
        <input
          type="date"
          name="initialDate"
          value={initialDate}
          onChange={(e) => onChange(e)}
        />
      </div>

      <div className="form-group">
        <label>End Date:</label>
        <input
          type="date"
          name="endDate"
          value={endDate}
          onChange={(e) => onChange(e)}
        />
      </div>

      <div className="form-group">
        <label>Date Type:</label>
        <select name="dateType" value={dateType} onChange={(e) => onChange(e)}>
          <option value="">Select Date Type</option>
          <option value="specific">Specific</option>
          <option value="range">Range</option>
        </select>
      </div>
    </div>
  );
};

export default HomeInputFields;
