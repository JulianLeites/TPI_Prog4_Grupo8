using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<IEnumerable<RoleDto>> GetAllAsync()
    {
        var roles = await _roleRepository.GetAllAsync();

        return roles.Select(role => new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
        });

    }

    public async Task<RoleDto?> GetByIdAsync(int id)
    {
        var role = await _roleRepository.GetByIdAsync(id);

        if (role is null)
            return null;

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
        };
    }

    public async Task<RoleDto?> UpdateAsync(
        int id,
        RoleUpdateDto dto
        )
    {
        var role = await _roleRepository.GetByIdAsync(id);

        if (role is null)
            return null;

        role.Name = dto.Name;

        await _roleRepository.UpdateAsync(role);

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var role = await _roleRepository.GetByIdAsync(id);

        if (role is null)
            return false;

        await _roleRepository.DeleteAsync(role);
        return true;
    }

    public async Task<RoleDto> CreateAsync(RoleCreateDto dto)
    {
        var role = new Role
        {
            Name = dto.Name
        };

        await _roleRepository.AddAsync(role);

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
        };
    }
}
